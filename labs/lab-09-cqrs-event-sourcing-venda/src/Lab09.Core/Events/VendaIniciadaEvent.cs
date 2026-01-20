namespace Lab09.Core.Events;

/// <summary>
/// Evento: Venda foi iniciada
/// </summary>
public sealed record VendaIniciadaEvent : DomainEventBase
{
    public Guid VendaId { get; init; }
    public Guid ClienteId { get; init; }
    public DateTime DataInicio { get; init; }
}
