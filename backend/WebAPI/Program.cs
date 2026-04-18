using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using WebAPI.Configuration;
using Microsoft.EntityFrameworkCore; // Required for UseNpgsql

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

// Register AppDbContext only if EntityFramework is selected
var crudImpl = builder.Configuration["CrudFactory:Implementation"]?.ToLower() ?? "inmemory";
if (crudImpl == "entityframework")
{
    var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<EntityFramework.AppDbContext>(options =>
        options.UseNpgsql(connStr));
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();

app.Run();
