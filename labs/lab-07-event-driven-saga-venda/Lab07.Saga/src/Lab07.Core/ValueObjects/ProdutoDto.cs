namespace Lab07.Core.ValueObjects;

/// <summary>
/// DTO para transferência de dados de Produto
/// </summary>
public record ProdutoDto
{
    public Guid Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public decimal Preco { get; init; }
    public int Estoque { get; init; }
    public int EstoqueReservado { get; init; }
    public int EstoqueDisponivel { get; init; }
    public bool Ativo { get; init; }
    public DateTime Created { get; init; }
}
