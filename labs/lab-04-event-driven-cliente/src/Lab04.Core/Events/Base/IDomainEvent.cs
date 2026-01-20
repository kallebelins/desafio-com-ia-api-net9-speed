namespace Lab04.Core.Events.Base;

/// <summary>
/// Interface base para eventos de domínio
/// </summary>
public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
    string EventType { get; }
}
