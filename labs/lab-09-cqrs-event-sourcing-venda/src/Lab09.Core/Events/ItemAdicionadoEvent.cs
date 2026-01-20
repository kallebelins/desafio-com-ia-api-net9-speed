namespace Lab09.Core.Events;

/// <summary>
/// Evento: Item foi adicionado à venda
/// </summary>
public sealed record ItemAdicionadoEvent : DomainEventBase
{
    public Guid VendaId { get; init; }
    public Guid ProdutoId { get; init; }
    public string ProdutoNome { get; init; } = string.Empty;
    public int Quantidade { get; init; }
    public decimal PrecoUnitario { get; init; }
}
