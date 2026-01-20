using System.Text.Json;
using Lab07.Core.Enums;
using Lab07.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lab07.Infrastructure.Outbox;

/// <summary>
/// Implementação do serviço de outbox
/// </summary>
public class OutboxService : IOutboxService
{
    private readonly DataContext _context;
    private readonly ILogger<OutboxService> _logger;

    public OutboxService(
        DataContext context,
        ILogger<OutboxService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddMessageAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            EventType = typeof(T).AssemblyQualifiedName ?? typeof(T).FullName ?? typeof(T).Name,
            Payload = JsonSerializer.Serialize(message),
            CreatedAt = DateTime.UtcNow,
            Status = OutboxStatus.Pendente,
            RetryCount = 0
        };

        await _context.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogDebug(
            "Mensagem adicionada ao outbox: {MessageId}, Tipo: {EventType}",
            outboxMessage.Id, typeof(T).Name);
    }

    public async Task<IEnumerable<OutboxMessage>> GetPendingMessagesAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        return await _context.OutboxMessages
            .Where(m => m.Status == OutboxStatus.Pendente && m.RetryCount < 5)
            .OrderBy(m => m.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAsPublishedAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await _context.OutboxMessages.FindAsync(new object[] { messageId }, cancellationToken);
        
        if (message == null) return;

        message.Status = OutboxStatus.Publicado;
        message.PublishedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogDebug(
            "Mensagem do outbox publicada: {MessageId}",
            messageId);
    }

    public async Task MarkAsFailedAsync(Guid messageId, string error, CancellationToken cancellationToken = default)
    {
        var message = await _context.OutboxMessages.FindAsync(new object[] { messageId }, cancellationToken);
        
        if (message == null) return;

        message.RetryCount++;
        message.Error = error;

        if (message.RetryCount >= 5)
        {
            message.Status = OutboxStatus.Falha;
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogWarning(
            "Mensagem do outbox falhou: {MessageId}, Tentativa: {RetryCount}, Erro: {Error}",
            messageId, message.RetryCount, error);
    }
}
