using Lab04.Core.Events.Base;

namespace Lab04.Core.Events.Integration;

/// <summary>
/// Evento de integração publicado no RabbitMQ quando um cliente é criado
/// Usado para comunicação entre serviços (ex: envio de email de boas-vindas)
/// </summary>
public record ClienteCriadoIntegrationEvent : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    public string EventType => nameof(ClienteCriadoIntegrationEvent);
    public string CorrelationId { get; init; } = string.Empty;
    
    public int ClienteId { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
