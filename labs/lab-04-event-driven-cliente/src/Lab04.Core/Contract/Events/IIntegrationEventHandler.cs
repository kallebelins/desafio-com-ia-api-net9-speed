using Lab04.Core.Events.Base;

namespace Lab04.Core.Contract.Events;

/// <summary>
/// Interface para handlers de eventos de integração
/// </summary>
public interface IIntegrationEventHandler<TEvent> where TEvent : IIntegrationEvent
{
    Task HandleAsync(TEvent integrationEvent, CancellationToken cancellationToken = default);
}
