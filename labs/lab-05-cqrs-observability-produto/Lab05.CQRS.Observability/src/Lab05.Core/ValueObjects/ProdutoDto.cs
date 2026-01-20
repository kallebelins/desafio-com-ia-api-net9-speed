namespace Lab05.Core.ValueObjects;

/// <summary>
/// DTO para transferência de dados do Produto
/// </summary>
public record ProdutoDto
{
    public Guid Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public decimal Preco { get; init; }
    public string SKU { get; init; } = string.Empty;
    public string Categoria { get; init; } = string.Empty;
    public bool Ativo { get; init; }
    public DateTime DataCriacao { get; init; }
    public DateTime? DataAtualizacao { get; init; }
}

/// <summary>
/// DTO para criação de produto
/// </summary>
public record CreateProdutoDto
{
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public decimal Preco { get; init; }
    public string SKU { get; init; } = string.Empty;
    public string Categoria { get; init; } = string.Empty;
}

/// <summary>
/// DTO para atualização de produto
/// </summary>
public record UpdateProdutoDto
{
    public Guid Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public decimal Preco { get; init; }
    public string SKU { get; init; } = string.Empty;
    public string Categoria { get; init; } = string.Empty;
    public bool Ativo { get; init; }
}
