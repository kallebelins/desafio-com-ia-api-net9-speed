using Lab09.Infrastructure.Projections;

namespace Lab09.WebAPI.HostedServices;

/// <summary>
/// Hosted Service para processar projeções em background
/// </summary>
public class ProjectionHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ProjectionHostedService> _logger;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(1);

    public ProjectionHostedService(
        IServiceProvider serviceProvider,
        ILogger<ProjectionHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Projection Hosted Service iniciado");

        // Aguardar um pouco para que o banco seja criado
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var projectionEngine = scope.ServiceProvider.GetRequiredService<ProjectionEngine>();

                await projectionEngine.ProcessPendingEventsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar projeções");
            }

            await Task.Delay(_pollingInterval, stoppingToken);
        }

        _logger.LogInformation("Projection Hosted Service encerrado");
    }
}
