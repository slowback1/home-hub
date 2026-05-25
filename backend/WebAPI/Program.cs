using System.Text.Json.Serialization;
using Common.Interfaces;
using Common.Models;
using Hangfire;
using InMemory;
using Logic.ActivityPicker;
using Logic.FeatureFlags;
using Logic.ComfyUi;
using Logic.Ollama;
using Logic.SystemConfig;
using Logic.Recurrence;
using Logic.Audiobook;
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
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.SnakeCaseLower)));
CrudFactoryConfigurator.ConfigureCrudFactory(builder.Services, builder.Configuration);
builder.Services.AddScoped<ActivityPickerJob>();
builder.Services.AddSingleton<IRecurrenceCalculator, IntervalDaysRecurrenceCalculator>();
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
        ["WEATHER_ENABLED"] = true,
        ["AUDIOBOOK_ENABLED"] = false,
        ["COMFYUI_ENABLED"] = false
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
        },
        new()
        {
            Id = "audiobook::provider", Namespace = "audiobook", Key = "provider", Value = "mock", Type = "select", IsSecret = false,
            Options =
            [
                new SystemConfigOption { SystemConfigId = "audiobook::provider", Value = "mock", Label = "Mock" },
                new SystemConfigOption { SystemConfigId = "audiobook::provider", Value = "gpu-service", Label = "GPU Service" }
            ]
        },
        new() { Id = "audiobook::url",     Namespace = "audiobook", Key = "url",     Value = "",     Type = "string", IsSecret = false },
        new() { Id = "audiobook::api_key", Namespace = "audiobook", Key = "api_key", Value = "",     Type = "string", IsSecret = true },
        new() { Id = "ollama::url",            Namespace = "ollama",    Key = "url",       Value = "",         Type = "string", IsSecret = false },
        new() { Id = "activity::selector",     Namespace = "activity",  Key = "selector",  Value = "random",   Type = "string", IsSecret = false },
        new() { Id = "activity::ai_model",     Namespace = "activity",  Key = "ai_model",  Value = "llama3.2", Type = "string", IsSecret = false },
        new() { Id = "comfyui::base_url",       Namespace = "comfyui",   Key = "base_url",  Value = "http://localhost:8199", Type = "string", IsSecret = false }
    };
    builder.Services.AddSingleton<ISystemConfigProvider>(new DictionarySystemConfigProvider(e2eEntries));
}

// Audiobook service — singleton mock or scoped GPU client based on audiobook::provider config value
builder.Services.AddSingleton<MockAudiobookService>();
builder.Services.AddHttpClient<GpuServiceClient>();
builder.Services.AddScoped<GpuServiceClient>();
builder.Services.AddScoped<IAudiobookService>(sp =>
{
    var cfg = sp.GetRequiredService<ISystemConfigProvider>();
    string key;
    try { key = cfg.GetAsync("audiobook", "provider").GetAwaiter().GetResult().Value; }
    catch { key = "mock"; }
    return key == "gpu-service"
        ? sp.GetRequiredService<GpuServiceClient>()
        : sp.GetRequiredService<MockAudiobookService>();
});

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

// ComfyUI client
builder.Services.AddHttpClient<ComfyUiClient>()
    .ConfigureHttpClient(c => c.Timeout = TimeSpan.FromSeconds(120));
builder.Services.AddScoped<IComfyUiClient>(sp => sp.GetRequiredService<ComfyUiClient>());

// Ollama client
builder.Services.AddHttpClient<OllamaClient>()
    .ConfigureHttpClient(c => c.Timeout = TimeSpan.FromSeconds(60));
builder.Services.AddScoped<IOllamaClient>(sp => sp.GetRequiredService<OllamaClient>());

// Activity selector — resolved per-request based on activity::selector config value
builder.Services.AddScoped<RandomActivitySelector>();
builder.Services.AddScoped<AiActivitySelector>();
builder.Services.AddScoped<IActivitySelector>(sp =>
{
    string key = "random";
    try
    {
        var cfg = sp.GetService<ISystemConfigProvider>();
        if (cfg != null)
            key = cfg.GetAsync("activity", "selector").GetAwaiter().GetResult().Value;
    }
    catch { key = "random"; }
    return key == "ai"
        ? sp.GetRequiredService<AiActivitySelector>()
        : sp.GetRequiredService<RandomActivitySelector>();
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
