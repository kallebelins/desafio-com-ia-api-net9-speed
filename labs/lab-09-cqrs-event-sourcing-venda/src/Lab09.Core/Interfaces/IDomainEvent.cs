namespace Lab09.Core.Interfaces;

/// <summary>
/// Interface base para eventos de domínio
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Identificador único do evento
    /// </summary>
    Guid EventId { get; }
    
    /// <summary>
    /// Data/hora em que o evento ocorreu
    /// </summary>
    DateTime OccurredAt { get; }
    
    /// <summary>
    /// Versão do evento (para versionamento de schema)
    /// </summary>
    int EventVersion { get; }
}
