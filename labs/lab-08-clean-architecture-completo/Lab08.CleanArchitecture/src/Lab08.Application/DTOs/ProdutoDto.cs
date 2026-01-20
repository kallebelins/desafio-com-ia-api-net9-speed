namespace Lab08.Application.DTOs;

/// <summary>
/// DTO para Produto
/// </summary>
public record ProdutoDto
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public decimal Preco { get; init; }
    public string PrecoFormatado { get; init; } = string.Empty;
    public int Estoque { get; init; }
    public bool Ativo { get; init; }
    public int CategoriaId { get; init; }
    public string CategoriaNome { get; init; } = string.Empty;
    public DateTime DataCadastro { get; init; }
    public DateTime? DataAtualizacao { get; init; }
}

/// <summary>
/// DTO resumido para Produto (listagens)
/// </summary>
public record ProdutoResumoDto
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public decimal Preco { get; init; }
    public string PrecoFormatado { get; init; } = string.Empty;
    public int Estoque { get; init; }
    public bool Ativo { get; init; }
}

/// <summary>
/// DTO para produto mais vendido (relatórios)
/// </summary>
public record ProdutoMaisVendidoDto
{
    public int ProdutoId { get; init; }
    public string ProdutoNome { get; init; } = string.Empty;
    public int QuantidadeVendida { get; init; }
    public decimal ValorTotalVendido { get; init; }
    public string ValorTotalFormatado { get; init; } = string.Empty;
}
