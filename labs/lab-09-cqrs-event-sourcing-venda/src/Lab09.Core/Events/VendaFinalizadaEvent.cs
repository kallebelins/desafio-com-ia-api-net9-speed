namespace Lab09.Core.Events;

/// <summary>
/// Evento: Venda foi finalizada com sucesso
/// </summary>
public sealed record VendaFinalizadaEvent : DomainEventBase
{
    public Guid VendaId { get; init; }
    public decimal TotalFinal { get; init; }
    public DateTime DataFinalizacao { get; init; }
}
