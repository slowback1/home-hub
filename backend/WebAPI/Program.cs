using Common.Interfaces;
using Common.Models;
using Hangfire;
using InMemory;
using Logic.ActivityPicker;
using Logic.FeatureFlags;
using Logic.SystemConfig;
using Logic.Weather;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using WebAPI.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

var serviceName = builder.Configuration["OpenTelemetry:ServiceName"] ?? "DotNetStarterKit.WebAPI";
var otlpEndpoint = builder.Configuration["OpenTelemetry:Otlp:Endpoint"] ?? "http://jaeger:4317";

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(options => { options.Endpoint = new Uri(otlpEndpoint); }));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
CrudFactoryConfigurator.ConfigureCrudFactory(builder.Services, builder.Configuration);
builder.Services.AddScoped<ActivityPickerJob>();
CorsConfigurator.ConfigureCors(builder.Services, builder.Configuration);
HangfireConfigurator.ConfigureHangfire(builder.Services, builder.Configuration);

// Register AppDbContext and EF-backed services only if EntityFramework is selected
var crudImpl = builder.Configuration["CrudFactory:Implementation"]?.ToLower() ?? "inmemory";
if (crudImpl == "entityframework")
{
    var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<EntityFramework.AppDbContext>(options =>
        options.UseNpgsql(connStr));
    builder.Services.AddScoped<ISystemConfigProvider>(sp =>
        new EntityFramework.EfSystemConfigProvider(sp.GetRequiredService<EntityFramework.AppDbContext>()));
    builder.Services.AddScoped<IActivityPickRepository, EntityFramework.EfActivityPickRepository>();
    builder.Services.AddScoped<IFeatureFlagProvider, EntityFramework.EntityFrameworkFeatureFlagProvider>();
    builder.Services.AddScoped<IFeatureFlagRepository, EntityFramework.EntityFrameworkFeatureFlagRepository>();
    builder.Services.AddScoped<UpdateFeatureFlagUseCase>();
}
else
{
    builder.Services.AddSingleton<IActivityPickRepository, InMemoryActivityPickRepository>();
    var inMemoryFlagStore = new DictionaryFeatureFlagProvider(new System.Collections.Generic.Dictionary<string, bool>
    {
        ["DEMO_FEATURE_FLAG"] = false,
        ["CHORE_TASK_TRACKER_ENABLED"] = true,
        ["ACTIVITY_PICKER_ENABLED"] = true,
        ["RETRO_ACHIEVEMENTS_ENABLED"] = true,
        ["WEATHER_ENABLED"] = true
    });
    builder.Services.AddSingleton<IFeatureFlagProvider>(inMemoryFlagStore);
    builder.Services.AddSingleton<IFeatureFlagRepository>(inMemoryFlagStore);
    builder.Services.AddSingleton<UpdateFeatureFlagUseCase>();
}

if (builder.Environment.IsEnvironment("E2E"))
{
    var e2eEntries = new List<SystemConfig>
    {
        new() { Id = "weather::zip_code", Namespace = "weather", Key = "zip_code", Value = "10001", Type = "text", IsSecret = false },
        new() { Id = "weather::api_key",  Namespace = "weather", Key = "api_key",  Value = "test-api-key-1", Type = "secret", IsSecret = true },
        new()
        {
            Id = "weather::provider", Namespace = "weather", Key = "provider", Value = "mock", Type = "select", IsSecret = false,
            Options =
            [
                new SystemConfigOption { SystemConfigId = "weather::provider", Value = "mock", Label = "Mock" },
                new SystemConfigOption { SystemConfigId = "weather::provider", Value = "openweathermap", Label = "Open Weather Map" }
            ]
        },
        new()
        {
            Id = "weather::units", Namespace = "weather", Key = "units", Value = "imperial", Type = "select", IsSecret = false,
            Options =
            [
                new SystemConfigOption { SystemConfigId = "weather::units", Value = "imperial", Label = "Imperial (°F, mph)" },
                new SystemConfigOption { SystemConfigId = "weather::units", Value = "metric", Label = "Metric (°C, km/h)" }
            ]
        }
    };
    builder.Services.AddSingleton<ISystemConfigProvider>(new DictionarySystemConfigProvider(e2eEntries));
}

// Weather provider — resolved per-request based on weather::provider config value
builder.Services.AddMemoryCache();
builder.Services.AddScoped<WeatherService>();
builder.Services.AddScoped<MockWeatherProvider>();
builder.Services.AddHttpClient<OpenWeatherMapProvider>();
builder.Services.AddScoped<OpenWeatherMapProvider>();
builder.Services.AddScoped<IWeatherProvider>(sp =>
{
    var cfg = sp.GetRequiredService<ISystemConfigProvider>();
    string key;
    try { key = cfg.GetAsync("weather", "provider").GetAwaiter().GetResult().Value; }
    catch { key = "mock"; }
    return key == "openweathermap"
        ? sp.GetRequiredService<OpenWeatherMapProvider>()
        : sp.GetRequiredService<MockWeatherProvider>();
});

var app = builder.Build();

if (crudImpl == "entityframework")
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<EntityFramework.AppDbContext>();
        db.Database.Migrate();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CorsConfigurator.PolicyName);
app.UseHangfireDashboard("/admin/hangfire", HangfireConfigurator.DashboardOptions);
RecurringJob.AddOrUpdate<ActivityPickerJob>("activity-picker", j => j.ExecuteAsync(), "0 * * * *");
app.MapControllers();
app.UseHttpsRedirection();

app.Run();
