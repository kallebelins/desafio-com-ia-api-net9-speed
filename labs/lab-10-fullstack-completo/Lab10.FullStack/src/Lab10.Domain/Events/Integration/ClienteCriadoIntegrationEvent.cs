namespace Lab10.Domain.Events.Integration;

/// <summary>
/// Evento de integração: Cliente criado (para outros serviços)
/// </summary>
public record ClienteCriadoIntegrationEvent(
    int ClienteId,
    string Nome,
    string Email,
    DateTime OcorridoEm) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
}
