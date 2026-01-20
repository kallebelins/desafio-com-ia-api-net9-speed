using Mvp24Hours.Core.Entities;
using Lab10.Domain.ValueObjects;
using Lab10.Domain.Enums;
using Lab10.Domain.Exceptions;

namespace Lab10.Domain.Entities;

/// <summary>
/// Entidade Pagamento
/// </summary>
public class Pagamento : EntityBase<int>
{
    // Construtor protegido para EF Core
    protected Pagamento() { }

    public Pagamento(int vendaId, Money valor, MetodoPagamento metodo)
    {
        VendaId = vendaId;
        Valor = valor ?? throw new ArgumentNullException(nameof(valor));
        Metodo = metodo;
        Status = PagamentoStatus.Pendente;
        DataCriacao = DateTime.UtcNow;
    }

    public int VendaId { get; private set; }
    public Money Valor { get; private set; } = null!;
    public MetodoPagamento Metodo { get; private set; }
    public PagamentoStatus Status { get; private set; }
    public string? TransacaoId { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataProcessamento { get; private set; }
    public string? MotivoFalha { get; private set; }

    // Navigation property
    public Venda? Venda { get; private set; }

    public void ProcessarComSucesso(string transacaoId)
    {
        if (Status != PagamentoStatus.Pendente && Status != PagamentoStatus.Processando)
            throw new DomainException("Pagamento não pode ser processado neste status");

        TransacaoId = transacaoId ?? throw new ArgumentNullException(nameof(transacaoId));
        Status = PagamentoStatus.Aprovado;
        DataProcessamento = DateTime.UtcNow;
    }

    public void IniciarProcessamento()
    {
        if (Status != PagamentoStatus.Pendente)
            throw new DomainException("Só é possível iniciar processamento de pagamentos pendentes");

        Status = PagamentoStatus.Processando;
    }

    public void Rejeitar(string motivo)
    {
        if (Status != PagamentoStatus.Pendente && Status != PagamentoStatus.Processando)
            throw new DomainException("Pagamento não pode ser rejeitado neste status");

        MotivoFalha = motivo;
        Status = PagamentoStatus.Rejeitado;
        DataProcessamento = DateTime.UtcNow;
    }

    public void Estornar(string motivo)
    {
        if (Status != PagamentoStatus.Aprovado)
            throw new DomainException("Só é possível estornar pagamentos aprovados");

        MotivoFalha = motivo;
        Status = PagamentoStatus.Estornado;
    }
}
