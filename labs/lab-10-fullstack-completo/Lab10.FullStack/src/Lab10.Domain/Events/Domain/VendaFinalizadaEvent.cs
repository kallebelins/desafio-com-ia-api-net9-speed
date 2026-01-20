namespace Lab10.Domain.Events.Domain;

/// <summary>
/// Evento de domínio: Venda finalizada
/// </summary>
public record VendaFinalizadaEvent(
    int VendaId,
    int ClienteId,
    decimal ValorTotal,
    DateTime OcorridoEm) : IDomainEvent
{
    public VendaFinalizadaEvent(int vendaId, int clienteId, decimal valorTotal)
        : this(vendaId, clienteId, valorTotal, DateTime.UtcNow) { }
}
