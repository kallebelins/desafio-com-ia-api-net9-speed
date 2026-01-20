using Lab04.Core.Events.Base;

namespace Lab04.Core.Contract.Events;

/// <summary>
/// Interface para despachar eventos de domínio para seus handlers
/// </summary>
public interface IDomainEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent;
    
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
