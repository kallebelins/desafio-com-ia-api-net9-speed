using Lab04.Core.Events.Base;

namespace Lab04.Core.Events.Domain;

/// <summary>
/// Evento de domínio disparado quando um cliente é excluído
/// </summary>
public record ClienteExcluidoEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventType => nameof(ClienteExcluidoEvent);
    
    public int ClienteId { get; init; }
}
