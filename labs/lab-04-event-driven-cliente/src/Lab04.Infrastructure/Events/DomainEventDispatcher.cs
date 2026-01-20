using Lab04.Core.Contract.Events;
using Lab04.Core.Events.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lab04.Infrastructure.Events;

/// <summary>
/// Implementação do dispatcher de eventos de domínio
/// Responsável por localizar e executar os handlers registrados
/// </summary>
public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DomainEventDispatcher> _logger;

    public DomainEventDispatcher(
        IServiceProvider serviceProvider,
        ILogger<DomainEventDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent
    {
        _logger.LogInformation("Dispatching domain event: {EventType} - EventId: {EventId}", 
            domainEvent.EventType, domainEvent.EventId);

        var handlers = _serviceProvider.GetServices<IDomainEventHandler<TEvent>>();
        
        foreach (var handler in handlers)
        {
            _logger.LogDebug("Executing handler {HandlerType} for event {EventType}",
                handler.GetType().Name, domainEvent.EventType);
            
            await handler.HandleAsync(domainEvent, cancellationToken);
        }

        _logger.LogInformation("Domain event dispatched successfully: {EventType}", domainEvent.EventType);
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            var eventType = domainEvent.GetType();
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
            var handlers = _serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                var method = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync));
                if (method != null)
                {
                    _logger.LogDebug("Executing handler {HandlerType} for event {EventType}",
                        handler?.GetType().Name, domainEvent.EventType);
                    
                    await (Task)method.Invoke(handler, new object[] { domainEvent, cancellationToken })!;
                }
            }
        }
    }
}
