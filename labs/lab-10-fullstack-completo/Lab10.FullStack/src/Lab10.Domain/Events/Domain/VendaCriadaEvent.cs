namespace Lab10.Domain.Events.Domain;

/// <summary>
/// Evento de domínio: Venda criada
/// </summary>
public record VendaCriadaEvent(
    int VendaId,
    int ClienteId,
    decimal ValorTotal,
    int QuantidadeItens,
    DateTime OcorridoEm) : IDomainEvent
{
    public VendaCriadaEvent(int vendaId, int clienteId, decimal valorTotal, int quantidadeItens)
        : this(vendaId, clienteId, valorTotal, quantidadeItens, DateTime.UtcNow) { }
}
