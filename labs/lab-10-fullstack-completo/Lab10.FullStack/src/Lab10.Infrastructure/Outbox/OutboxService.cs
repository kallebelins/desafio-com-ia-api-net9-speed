using System.Text.Json;
using Lab10.Application.Interfaces;
using Lab10.Infrastructure.Data;

namespace Lab10.Infrastructure.Outbox;

/// <summary>
/// Serviço de Outbox para garantia de entrega de eventos
/// </summary>
public class OutboxService : IOutboxService
{
    private readonly WriteDbContext _context;

    public OutboxService(WriteDbContext context)
    {
        _context = context;
    }

    public async Task AddMessageAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            EventType = typeof(T).AssemblyQualifiedName ?? typeof(T).Name,
            Payload = JsonSerializer.Serialize(message),
            CreatedAt = DateTime.UtcNow,
            Status = OutboxStatus.Pending,
            RetryCount = 0
        };

        await _context.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    }
}
