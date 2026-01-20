namespace Lab10.Domain.Events.Domain;

/// <summary>
/// Evento de domínio: Venda cancelada
/// </summary>
public record VendaCanceladaEvent(
    int VendaId,
    int ClienteId,
    string? Motivo,
    DateTime OcorridoEm) : IDomainEvent
{
    public VendaCanceladaEvent(int vendaId, int clienteId, string? motivo)
        : this(vendaId, clienteId, motivo, DateTime.UtcNow) { }
}
