using Lab04.Core.Contract.Events;
using Lab04.Core.Events.Domain;
using Lab04.Core.Events.Integration;
using Microsoft.Extensions.Logging;

namespace Lab04.Application.EventHandlers.Domain;

/// <summary>
/// Handler do evento de domínio ClienteCriado
/// Responsável por publicar o evento de integração no RabbitMQ
/// </summary>
public class ClienteCriadoEventHandler : IDomainEventHandler<ClienteCriadoEvent>
{
    private readonly IIntegrationEventPublisher _eventPublisher;
    private readonly ILogger<ClienteCriadoEventHandler> _logger;

    public ClienteCriadoEventHandler(
        IIntegrationEventPublisher eventPublisher,
        ILogger<ClienteCriadoEventHandler> logger)
    {
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task HandleAsync(ClienteCriadoEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Handling ClienteCriadoEvent for ClienteId: {ClienteId}, Nome: {Nome}",
            domainEvent.ClienteId, domainEvent.Nome);

        // Converter para evento de integração e publicar no RabbitMQ
        var integrationEvent = new ClienteCriadoIntegrationEvent
        {
            ClienteId = domainEvent.ClienteId,
            Nome = domainEvent.Nome,
            Email = domainEvent.Email,
            CorrelationId = domainEvent.EventId.ToString()
        };

        await _eventPublisher.PublishAsync(integrationEvent, "cliente.criado", cancellationToken);

        _logger.LogInformation(
            "ClienteCriadoIntegrationEvent published for ClienteId: {ClienteId}",
            domainEvent.ClienteId);
    }
}
