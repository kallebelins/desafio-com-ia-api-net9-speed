using NLog;
using NLog.Web;
using Lab10.WebAPI.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    logger.Debug("Iniciando aplicação Lab10 Full Stack...");

    var builder = WebApplication.CreateBuilder(args);

    // NLog
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // Configurar serviços
    builder.Services.AddMyServices(builder.Configuration);
    builder.Services.AddObservability(builder.Configuration);

    // Controllers
    builder.Services.AddControllers();

    // Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() 
        { 
            Title = "Lab10 Full Stack - Sistema Completo", 
            Version = "v1",
            Description = "API Full Stack com CQRS, Event-Driven, Saga, Observability e Clean Architecture"
        });
    });

    var app = builder.Build();

    // Configurar pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lab10 Full Stack v1"));
    }

    app.UseMiddlewares();

    app.UseAuthorization();

    app.MapControllers();

    // Health Checks
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        Predicate = _ => true
    });

    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        Predicate = check => check.Tags.Contains("ready")
    });

    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        Predicate = _ => false
    });

    // Prometheus metrics endpoint
    app.MapPrometheusScrapingEndpoint("/metrics");

    // Aplicar migrations
    await app.ApplyMigrationsAsync();

    logger.Info("Aplicação iniciada com sucesso");
    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Aplicação parou devido a uma exceção");
    throw;
}
finally
{
    LogManager.Shutdown();
}
