using Lab04.Core.Events.Base;

namespace Lab04.Core.Contract.Events;

/// <summary>
/// Interface para handlers de eventos de domínio
/// </summary>
public interface IDomainEventHandler<TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}
