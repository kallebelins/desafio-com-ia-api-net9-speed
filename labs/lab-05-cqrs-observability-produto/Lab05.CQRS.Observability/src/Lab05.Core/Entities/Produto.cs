using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mvp24Hours.Core.Contract.Domain.Entity;

namespace Lab05.Core.Entities;

/// <summary>
/// Entidade Produto com campos: Id, Nome, Descrição, Preço, SKU, Categoria, Ativo
/// </summary>
public class Produto : IEntityBase
{
    public Produto()
    {
        Id = Guid.NewGuid();
        Ativo = true;
        DataCriacao = DateTime.UtcNow;
    }

    /// <summary>
    /// Identificador único do produto
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    /// <summary>
    /// Nome do produto
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Descrição detalhada do produto
    /// </summary>
    [MaxLength(1000)]
    public string? Descricao { get; set; }

    /// <summary>
    /// Preço do produto
    /// </summary>
    [Required]
    public decimal Preco { get; set; }

    /// <summary>
    /// Stock Keeping Unit - código único do produto
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string SKU { get; set; } = string.Empty;

    /// <summary>
    /// Categoria do produto
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Categoria { get; set; } = string.Empty;

    /// <summary>
    /// Indica se o produto está ativo
    /// </summary>
    public bool Ativo { get; set; }

    /// <summary>
    /// Data de criação do registro
    /// </summary>
    public DateTime DataCriacao { get; set; }

    /// <summary>
    /// Data da última atualização
    /// </summary>
    public DateTime? DataAtualizacao { get; set; }

    /// <summary>
    /// Data de remoção (soft delete)
    /// </summary>
    public DateTime? DataRemocao { get; set; }

    // Implementação de IEntityBase
    object IEntityBase.EntityKey => Id;
}
