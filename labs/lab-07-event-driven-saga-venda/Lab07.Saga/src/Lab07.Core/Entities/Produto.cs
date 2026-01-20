using Mvp24Hours.Core.Contract.Domain.Entity;
using Mvp24Hours.Core.Entities;

namespace Lab07.Core.Entities;

/// <summary>
/// Entidade Produto
/// </summary>
public class Produto : EntityBase<Guid>, IEntityDateLog
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public int EstoqueReservado { get; set; }
    public bool Ativo { get; set; } = true;

    // Navegação
    public virtual ICollection<ItemVenda> ItensVenda { get; set; } = new List<ItemVenda>();

    // IEntityDateLog
    public DateTime Created { get; set; }
    public DateTime? Modified { get; set; }
    public DateTime? Removed { get; set; }

    /// <summary>
    /// Estoque disponível (estoque - reservado)
    /// </summary>
    public int EstoqueDisponivel => Estoque - EstoqueReservado;

    /// <summary>
    /// Reserva estoque para uma venda
    /// </summary>
    public bool ReservarEstoque(int quantidade)
    {
        if (EstoqueDisponivel < quantidade)
            return false;

        EstoqueReservado += quantidade;
        return true;
    }

    /// <summary>
    /// Libera estoque reservado
    /// </summary>
    public void LiberarReserva(int quantidade)
    {
        EstoqueReservado = Math.Max(0, EstoqueReservado - quantidade);
    }

    /// <summary>
    /// Confirma a reserva, diminuindo o estoque real
    /// </summary>
    public void ConfirmarReserva(int quantidade)
    {
        Estoque = Math.Max(0, Estoque - quantidade);
        EstoqueReservado = Math.Max(0, EstoqueReservado - quantidade);
    }
}
