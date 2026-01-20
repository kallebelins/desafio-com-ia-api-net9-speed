namespace Lab07.Core.ValueObjects;

/// <summary>
/// Request para criar uma nova venda
/// </summary>
public record CriarVendaRequest
{
    public Guid ClienteId { get; init; }
    public List<ItemVendaRequest> Itens { get; init; } = new();
}

/// <summary>
/// Item de uma requisição de venda
/// </summary>
public record ItemVendaRequest
{
    public Guid ProdutoId { get; init; }
    public int Quantidade { get; init; }
}
