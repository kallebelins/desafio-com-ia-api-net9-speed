namespace Lab10.Domain.Events.Domain;

/// <summary>
/// Evento de domínio: Estoque reservado
/// </summary>
public record EstoqueReservadoEvent(
    int ProdutoId,
    int VendaId,
    int Quantidade,
    DateTime OcorridoEm) : IDomainEvent
{
    public EstoqueReservadoEvent(int produtoId, int vendaId, int quantidade)
        : this(produtoId, vendaId, quantidade, DateTime.UtcNow) { }
}
