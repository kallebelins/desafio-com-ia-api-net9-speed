namespace Lab09.Core.Events;

/// <summary>
/// Evento: Desconto foi aplicado à venda
/// </summary>
public sealed record DescontoAplicadoEvent : DomainEventBase
{
    public Guid VendaId { get; init; }
    public decimal ValorDesconto { get; init; }
    public decimal PercentualDesconto { get; init; }
    public string? MotivoDesconto { get; init; }
}
