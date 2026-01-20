using Lab07.Core.Enums;
using Mvp24Hours.Core.Contract.Domain.Entity;
using Mvp24Hours.Core.Entities;

namespace Lab07.Core.Entities;

/// <summary>
/// Entidade Venda
/// </summary>
public class Venda : EntityBase<Guid>, IEntityDateLog
{
    public Guid ClienteId { get; set; }
    public decimal ValorTotal { get; set; }
    public VendaStatus Status { get; set; } = VendaStatus.Pendente;
    public string? MotivoFalha { get; set; }
    public DateTime? DataConfirmacao { get; set; }
    public DateTime? DataCancelamento { get; set; }

    // Navegação
    public virtual Cliente? Cliente { get; set; }
    public virtual ICollection<ItemVenda> Itens { get; set; } = new List<ItemVenda>();

    // IEntityDateLog
    public DateTime Created { get; set; }
    public DateTime? Modified { get; set; }
    public DateTime? Removed { get; set; }

    /// <summary>
    /// Calcula o valor total da venda com base nos itens
    /// </summary>
    public void CalcularValorTotal()
    {
        ValorTotal = Itens.Sum(i => i.ValorTotal);
    }

    /// <summary>
    /// Confirma a venda
    /// </summary>
    public void Confirmar()
    {
        Status = VendaStatus.Confirmada;
        DataConfirmacao = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancela a venda
    /// </summary>
    public void Cancelar(string motivo)
    {
        Status = VendaStatus.Cancelada;
        MotivoFalha = motivo;
        DataCancelamento = DateTime.UtcNow;
    }

    /// <summary>
    /// Marca a venda como falha
    /// </summary>
    public void MarcarFalha(string motivo)
    {
        Status = VendaStatus.Falha;
        MotivoFalha = motivo;
    }
}
