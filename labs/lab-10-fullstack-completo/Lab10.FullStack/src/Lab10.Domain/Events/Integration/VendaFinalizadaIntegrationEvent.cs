namespace Lab10.Domain.Events.Integration;

/// <summary>
/// Evento de integração: Venda finalizada (para outros serviços - analytics, notificação)
/// </summary>
public record VendaFinalizadaIntegrationEvent(
    int VendaId,
    int ClienteId,
    string ClienteEmail,
    decimal ValorTotal,
    int QuantidadeItens,
    DateTime DataFinalizacao) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
}
