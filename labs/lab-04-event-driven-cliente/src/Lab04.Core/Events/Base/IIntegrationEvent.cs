namespace Lab04.Core.Events.Base;

/// <summary>
/// Interface base para eventos de integração (cross-service)
/// </summary>
public interface IIntegrationEvent
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
    string EventType { get; }
    string CorrelationId { get; }
}
