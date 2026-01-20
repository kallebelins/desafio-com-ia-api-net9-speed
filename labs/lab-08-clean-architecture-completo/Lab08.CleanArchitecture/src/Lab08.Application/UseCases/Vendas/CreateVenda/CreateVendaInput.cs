namespace Lab08.Application.UseCases.Vendas.CreateVenda;

/// <summary>
/// Input para criação de venda
/// </summary>
public record CreateVendaInput
{
    public int ClienteId { get; init; }
    public IReadOnlyList<ItemVendaInput> Itens { get; init; } = [];
    public string? Observacao { get; init; }
}

/// <summary>
/// Input para item de venda
/// </summary>
public record ItemVendaInput
{
    public int ProdutoId { get; init; }
    public int Quantidade { get; init; }
}
