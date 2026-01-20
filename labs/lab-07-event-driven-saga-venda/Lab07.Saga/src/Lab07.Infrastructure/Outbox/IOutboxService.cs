namespace Lab07.Infrastructure.Outbox;

/// <summary>
/// Interface do serviço de outbox
/// </summary>
public interface IOutboxService
{
    Task AddMessageAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
    Task<IEnumerable<OutboxMessage>> GetPendingMessagesAsync(int batchSize, CancellationToken cancellationToken = default);
    Task MarkAsPublishedAsync(Guid messageId, CancellationToken cancellationToken = default);
    Task MarkAsFailedAsync(Guid messageId, string error, CancellationToken cancellationToken = default);
}
