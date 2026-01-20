namespace Lab10.Domain.Events.Domain;

/// <summary>
/// Evento de domínio: Estoque liberado (compensação de reserva)
/// </summary>
public record EstoqueLiberadoEvent(
    int ProdutoId,
    int VendaId,
    int Quantidade,
    DateTime OcorridoEm) : IDomainEvent
{
    public EstoqueLiberadoEvent(int produtoId, int vendaId, int quantidade)
        : this(produtoId, vendaId, quantidade, DateTime.UtcNow) { }
}
