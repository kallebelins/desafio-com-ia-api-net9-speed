using Mvp24Hours.Core.Contract.Domain.Entity;
using Mvp24Hours.Core.Entities;

namespace Lab07.Core.Entities;

/// <summary>
/// Entidade Cliente
/// </summary>
public class Cliente : EntityBase<Guid>, IEntityDateLog
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;

    // Navegação
    public virtual ICollection<Venda> Vendas { get; set; } = new List<Venda>();

    // IEntityDateLog
    public DateTime Created { get; set; }
    public DateTime? Modified { get; set; }
    public DateTime? Removed { get; set; }
}
