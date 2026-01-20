namespace Lab09.Core.Events;

/// <summary>
/// Evento: Venda foi cancelada
/// </summary>
public sealed record VendaCanceladaEvent : DomainEventBase
{
    public Guid VendaId { get; init; }
    public string Motivo { get; init; } = string.Empty;
    public DateTime DataCancelamento { get; init; }
}
