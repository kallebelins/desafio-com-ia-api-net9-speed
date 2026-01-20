using Lab04.Core.Events.Base;

namespace Lab04.Core.Contract.Events;

/// <summary>
/// Interface para publicar eventos de integração no message broker (RabbitMQ)
/// </summary>
public interface IIntegrationEventPublisher
{
    Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent;
    
    Task PublishAsync<TEvent>(TEvent integrationEvent, string routingKey, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent;
}
