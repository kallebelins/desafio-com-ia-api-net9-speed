namespace Lab09.Core.Interfaces;

/// <summary>
/// Interface para Aggregate Roots com Event Sourcing
/// </summary>
public interface IAggregateRoot
{
    /// <summary>
    /// Identificador único do Aggregate
    /// </summary>
    Guid Id { get; }
    
    /// <summary>
    /// Versão atual do Aggregate (número de eventos aplicados)
    /// </summary>
    int Version { get; }
    
    /// <summary>
    /// Eventos não confirmados (não persistidos ainda)
    /// </summary>
    IReadOnlyList<IDomainEvent> GetUncommittedEvents();
    
    /// <summary>
    /// Limpa a lista de eventos não confirmados
    /// </summary>
    void ClearUncommittedEvents();
    
    /// <summary>
    /// Aplica um evento ao aggregate
    /// </summary>
    void ApplyEvent(IDomainEvent @event);
}
