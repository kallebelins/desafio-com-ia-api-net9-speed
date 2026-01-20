namespace Lab08.Application.DTOs;

/// <summary>
/// DTO para Categoria
/// </summary>
public record CategoriaDto
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public bool Ativo { get; init; }
    public int QuantidadeProdutos { get; init; }
}

/// <summary>
/// DTO resumido para Categoria (listagens/selects)
/// </summary>
public record CategoriaResumoDto
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
}
