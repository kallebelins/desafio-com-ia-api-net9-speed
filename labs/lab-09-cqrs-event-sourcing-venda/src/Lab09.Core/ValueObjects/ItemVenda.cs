namespace Lab09.Core.ValueObjects;

/// <summary>
/// Value Object que representa um item de venda
/// </summary>
public sealed record ItemVenda
{
    public Guid ProdutoId { get; }
    public string ProdutoNome { get; }
    public int Quantidade { get; }
    public decimal PrecoUnitario { get; }
    public decimal Subtotal => Quantidade * PrecoUnitario;

    public ItemVenda(Guid produtoId, string produtoNome, int quantidade, decimal precoUnitario)
    {
        if (produtoId == Guid.Empty)
            throw new ArgumentException("ProdutoId não pode ser vazio", nameof(produtoId));
        if (string.IsNullOrWhiteSpace(produtoNome))
            throw new ArgumentException("Nome do produto não pode ser vazio", nameof(produtoNome));
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantidade));
        if (precoUnitario < 0)
            throw new ArgumentException("Preço unitário não pode ser negativo", nameof(precoUnitario));

        ProdutoId = produtoId;
        ProdutoNome = produtoNome;
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
    }
}
