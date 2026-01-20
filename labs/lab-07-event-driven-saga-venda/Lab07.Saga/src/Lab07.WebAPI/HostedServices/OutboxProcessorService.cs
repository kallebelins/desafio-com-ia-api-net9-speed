using System.Text.Json;
using Lab07.Infrastructure.Outbox;

namespace Lab07.WebAPI.HostedServices;

/// <summary>
/// Serviço de background que processa mensagens do outbox e publica no RabbitMQ
/// </summary>
public class OutboxProcessorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessorService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(5);
    private readonly int _batchSize = 100;

    public OutboxProcessorService(
        IServiceProvider serviceProvider,
        ILogger<OutboxProcessorService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OutboxProcessorService iniciado");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar outbox");
            }

            await Task.Delay(_interval, stoppingToken);
        }

        _logger.LogInformation("OutboxProcessorService finalizado");
    }

    private async Task ProcessOutboxAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var outboxService = scope.ServiceProvider.GetRequiredService<IOutboxService>();

        var messages = await outboxService.GetPendingMessagesAsync(_batchSize, stoppingToken);
        var messageList = messages.ToList();

        if (messageList.Count == 0)
            return;

        _logger.LogDebug("Processando {Count} mensagens do outbox", messageList.Count);

        foreach (var message in messageList)
        {
            try
            {
                // Modo simulado - apenas loga (RabbitMQ seria integrado aqui)
                var eventTypeName = message.EventType.Split(',').First().Split('.').Last();
                _logger.LogInformation(
                    "[OUTBOX] Publicando evento {EventType}: {Payload}",
                    eventTypeName,
                    message.Payload.Length > 200 ? message.Payload[..200] + "..." : message.Payload);

                await outboxService.MarkAsPublishedAsync(message.Id, stoppingToken);

                _logger.LogDebug(
                    "Mensagem {MessageId} publicada com sucesso",
                    message.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Falha ao publicar mensagem {MessageId}",
                    message.Id);

                await outboxService.MarkAsFailedAsync(
                    message.Id,
                    ex.Message,
                    stoppingToken);
            }
        }
    }
}
