using Lab09.Core.Interfaces;

namespace Lab09.Core.Events;

/// <summary>
/// Classe base para eventos de domínio
/// </summary>
public abstract record DomainEventBase : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public int EventVersion { get; init; } = 1;
}
