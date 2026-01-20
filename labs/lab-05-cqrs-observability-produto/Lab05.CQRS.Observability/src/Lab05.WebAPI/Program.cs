using Lab05.WebAPI.Extensions;
using Lab05.WebAPI.Middlewares;
using NLog;
using NLog.Web;

// NLog: Setup logger for startup
var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    logger.Debug("Application starting...");

    var builder = WebApplication.CreateBuilder(args);

    // NLog
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // Controllers
    builder.Services.AddControllers();

    // Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new()
        {
            Title = "Lab05 CQRS + Observability - Produtos",
            Version = "v1",
            Description = "API REST para cadastro de produtos com CQRS e Observability completa"
        });
    });

    // Database
    builder.Services.AddDatabaseServices(builder.Configuration);

    // CQRS (Mvp24Hours)
    builder.Services.AddCqrsServices();

    // Observability (OpenTelemetry)
    builder.Services.AddObservabilityServices(builder.Configuration);

    // Health Checks
    builder.Services.AddHealthCheckServices(builder.Configuration);

    var app = builder.Build();

    // Middlewares
    app.UseExceptionHandling();
    app.UseCorrelationId();

    // Swagger
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lab05 CQRS + Observability v1");
            c.RoutePrefix = "swagger";
        });
    }

    // Observability Endpoints
    app.MapObservabilityEndpoints();

    // Controllers
    app.MapControllers();

    // Ensure database created
    await app.EnsureDatabaseCreatedAsync();

    logger.Info("Application started successfully");
    logger.Info("Swagger UI: http://localhost:5000/swagger");
    logger.Info("Health Check: http://localhost:5000/health");
    logger.Info("Metrics: http://localhost:5000/metrics");

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Application stopped due to exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}
