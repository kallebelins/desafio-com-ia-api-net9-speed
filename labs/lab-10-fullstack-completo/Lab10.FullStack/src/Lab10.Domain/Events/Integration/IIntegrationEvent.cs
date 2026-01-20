namespace Lab10.Domain.Events.Integration;

/// <summary>
/// Interface base para eventos de integração (cross-service)
/// </summary>
public interface IIntegrationEvent
{
    Guid EventId { get; }
}
