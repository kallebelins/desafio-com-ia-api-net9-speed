using Lab03.Core.Entities;

namespace Lab03.Core.ValueObjects;

/// <summary>
/// DTO para transferência de dados do Produto
/// </summary>
public class ProdutoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public int Estoque { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }

    /// <summary>
    /// Cria um DTO a partir da entidade
    /// </summary>
    public static ProdutoDto FromEntity(Produto produto)
    {
        return new ProdutoDto
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Preco = produto.Preco,
            Categoria = produto.Categoria,
            Estoque = produto.Estoque,
            Ativo = produto.Ativo,
            DataCriacao = produto.Created,
            DataAtualizacao = produto.Modified
        };
    }

    /// <summary>
    /// Cria uma lista de DTOs a partir de entidades
    /// </summary>
    public static IEnumerable<ProdutoDto> FromEntities(IEnumerable<Produto> produtos)
    {
        return produtos.Select(FromEntity);
    }
}
