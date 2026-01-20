using Mvp24Hours.Core.Contract.Domain.Entity;
using Mvp24Hours.Core.Entities;

namespace Lab07.Core.Entities;

/// <summary>
/// Entidade Item de Venda
/// </summary>
public class ItemVenda : EntityBase<Guid>, IEntityDateLog
{
    public Guid VendaId { get; set; }
    public Guid ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal ValorTotal => Quantidade * PrecoUnitario;

    // Navegação
    public virtual Venda? Venda { get; set; }
    public virtual Produto? Produto { get; set; }

    // IEntityDateLog
    public DateTime Created { get; set; }
    public DateTime? Modified { get; set; }
    public DateTime? Removed { get; set; }
}
