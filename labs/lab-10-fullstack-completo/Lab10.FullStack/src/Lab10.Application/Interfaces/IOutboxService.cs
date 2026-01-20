namespace Lab10.Application.Interfaces;

/// <summary>
/// Interface do serviço de Outbox para garantia de entrega de eventos
/// </summary>
public interface IOutboxService
{
    Task AddMessageAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
}
