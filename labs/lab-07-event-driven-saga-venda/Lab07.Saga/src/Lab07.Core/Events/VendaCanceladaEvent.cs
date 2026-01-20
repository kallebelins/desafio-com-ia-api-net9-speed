namespace Lab07.Core.Events;

/// <summary>
/// Evento disparado quando uma venda é cancelada
/// </summary>
public record VendaCanceladaEvent
{
    public Guid VendaId { get; init; }
    public Guid ClienteId { get; init; }
    public string Motivo { get; init; } = string.Empty;
    public DateTime OcorridoEm { get; init; } = DateTime.UtcNow;
}
