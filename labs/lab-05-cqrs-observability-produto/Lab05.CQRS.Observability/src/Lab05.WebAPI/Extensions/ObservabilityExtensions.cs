using HealthChecks.UI.Client;
using Lab05.Infrastructure.Observability;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Lab05.WebAPI.Extensions;

/// <summary>
/// Extensions para configuração de observabilidade
/// </summary>
public static class ObservabilityExtensions
{
    /// <summary>
    /// Configura OpenTelemetry (Tracing e Metrics)
    /// </summary>
    public static IServiceCollection AddObservabilityServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configurar OpenTelemetry
        services.AddOpenTelemetryObservability(configuration);

        return services;
    }

    /// <summary>
    /// Mapeia os endpoints de observabilidade
    /// </summary>
    public static WebApplication MapObservabilityEndpoints(this WebApplication app)
    {
        // Health Check endpoint principal
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            Predicate = _ => true
        });

        // Health Check para readiness (Kubernetes)
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            Predicate = check => check.Tags.Contains("ready")
        });

        // Health Check para liveness (Kubernetes)
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            Predicate = _ => false // Liveness apenas verifica se app está rodando
        });

        // Prometheus metrics endpoint
        app.MapPrometheusScrapingEndpoint("/metrics");

        return app;
    }
}
