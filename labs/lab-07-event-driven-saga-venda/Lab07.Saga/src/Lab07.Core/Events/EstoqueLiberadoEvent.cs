namespace Lab07.Core.Events;

/// <summary>
/// Evento disparado quando o estoque reservado é liberado (compensação)
/// </summary>
public record EstoqueLiberadoEvent
{
    public Guid VendaId { get; init; }
    public List<ReservaEstoqueData> Reservas { get; init; } = new();
    public string Motivo { get; init; } = string.Empty;
    public DateTime OcorridoEm { get; init; } = DateTime.UtcNow;
}
