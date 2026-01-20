namespace Lab09.Core.Interfaces;

/// <summary>
/// Interface para o Event Store - persistência de eventos
/// </summary>
public interface IEventStore
{
    /// <summary>
    /// Salva os eventos de um Aggregate
    /// </summary>
    /// <param name="aggregateId">ID do Aggregate</param>
    /// <param name="events">Eventos a serem salvos</param>
    /// <param name="expectedVersion">Versão esperada para controle de concorrência</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task SaveEventsAsync(
        Guid aggregateId, 
        IEnumerable<IDomainEvent> events, 
        int expectedVersion,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Obtém todos os eventos de um Aggregate
    /// </summary>
    /// <param name="aggregateId">ID do Aggregate</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de eventos ordenados por versão</returns>
    Task<IList<IDomainEvent>> GetEventsAsync(
        Guid aggregateId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Obtém eventos de um Aggregate a partir de uma versão específica
    /// </summary>
    /// <param name="aggregateId">ID do Aggregate</param>
    /// <param name="fromVersion">Versão inicial</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de eventos ordenados por versão</returns>
    Task<IList<IDomainEvent>> GetEventsFromVersionAsync(
        Guid aggregateId, 
        int fromVersion,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Obtém todos os eventos não processados para projeções
    /// </summary>
    /// <param name="lastProcessedPosition">Última posição processada</param>
    /// <param name="batchSize">Tamanho do lote</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de eventos com suas posições</returns>
    Task<IList<(IDomainEvent Event, long Position)>> GetUnprocessedEventsAsync(
        long lastProcessedPosition,
        int batchSize = 100,
        CancellationToken cancellationToken = default);
}
