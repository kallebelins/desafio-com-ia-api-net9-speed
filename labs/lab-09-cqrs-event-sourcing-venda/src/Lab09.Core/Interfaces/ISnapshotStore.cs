namespace Lab09.Core.Interfaces;

/// <summary>
/// Interface para armazenamento de Snapshots
/// </summary>
public interface ISnapshotStore
{
    /// <summary>
    /// Salva um snapshot do Aggregate
    /// </summary>
    /// <typeparam name="TAggregate">Tipo do Aggregate</typeparam>
    /// <param name="aggregate">Aggregate a ser salvo</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task SaveSnapshotAsync<TAggregate>(TAggregate aggregate, CancellationToken cancellationToken = default)
        where TAggregate : IAggregateRoot;
    
    /// <summary>
    /// Obtém o snapshot mais recente de um Aggregate
    /// </summary>
    /// <typeparam name="TAggregate">Tipo do Aggregate</typeparam>
    /// <param name="aggregateId">ID do Aggregate</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Snapshot ou null se não existir</returns>
    Task<TAggregate?> GetSnapshotAsync<TAggregate>(Guid aggregateId, CancellationToken cancellationToken = default)
        where TAggregate : IAggregateRoot;
}
