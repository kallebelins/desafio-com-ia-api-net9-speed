using Mvp24Hours.Core.Contract.Domain;
using Mvp24Hours.Core.Entities;

namespace Lab03.Core.Entities;

/// <summary>
/// Entidade Produto para o catálogo de produtos
/// </summary>
public class Produto : EntityBase<int>
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public int Estoque { get; set; }
    public bool Ativo { get; set; } = true;
    
    // Audit fields
    public DateTime Created { get; set; }
    public DateTime? Modified { get; set; }
}
