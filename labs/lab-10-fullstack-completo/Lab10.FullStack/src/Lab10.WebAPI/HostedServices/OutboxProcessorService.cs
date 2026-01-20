using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Lab10.Infrastructure.Data;

namespace Lab10.WebAPI.HostedServices;

/// <summary>
/// Hosted Service para processar mensagens do Outbox
/// </summary>
public class OutboxProcessorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessorService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(5);

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

        var timer = new PeriodicTimer(_interval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await ProcessOutboxAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar Outbox");
            }
        }

        _logger.LogInformation("OutboxProcessorService parado");
    }

    private async Task ProcessOutboxAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

        var messages = await context.OutboxMessages
            .Where(m => m.Status == OutboxStatus.Pending && m.RetryCount < 5)
            .OrderBy(m => m.CreatedAt)
            .Take(100)
            .ToListAsync(stoppingToken);

        if (!messages.Any())
            return;

        _logger.LogInformation("Processando {Count} mensagens do Outbox", messages.Count);

        foreach (var message in messages)
        {
            try
            {
                var eventTypeName = message.EventType.Split(',')[0].Split('.').Last();
                
                // Simular publicação (em produção, usaria RabbitMQ)
                _logger.LogInformation(
                    "[OUTBOX] Publicando evento {EventType}: {Payload}",
                    eventTypeName,
                    message.Payload.Length > 200 ? message.Payload[..200] + "..." : message.Payload);

                // Marcar como publicado
                message.Status = OutboxStatus.Published;
                message.PublishedAt = DateTime.UtcNow;

                _logger.LogInformation("[OUTBOX] Evento publicado com sucesso: {EventId}", message.Id);
            }
            catch (Exception ex)
            {
                message.RetryCount++;
                message.Error = ex.Message;

                if (message.RetryCount >= 5)
                {
                    message.Status = OutboxStatus.Failed;
                    _logger.LogError(ex, "[OUTBOX] Evento falhou após 5 tentativas: {EventId}", message.Id);
                }
                else
                {
                    _logger.LogWarning(ex, "[OUTBOX] Erro ao publicar evento (tentativa {RetryCount}): {EventId}", 
                        message.RetryCount, message.Id);
                }
            }
        }

        await context.SaveChangesAsync(stoppingToken);
    }
}
