namespace Lab10.Domain.Events.Domain;

/// <summary>
/// Evento de domínio: Pagamento processado
/// </summary>
public record PagamentoProcessadoEvent(
    int PagamentoId,
    int VendaId,
    decimal Valor,
    string? TransacaoId,
    bool Aprovado,
    string? MotivoFalha,
    DateTime OcorridoEm) : IDomainEvent
{
    public PagamentoProcessadoEvent(int pagamentoId, int vendaId, decimal valor, string? transacaoId, bool aprovado, string? motivoFalha = null)
        : this(pagamentoId, vendaId, valor, transacaoId, aprovado, motivoFalha, DateTime.UtcNow) { }
}
