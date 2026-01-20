namespace Lab09.Application.DTOs;

/// <summary>
/// DTO para representar um evento do histórico
/// </summary>
public record EventoDto
{
    public Guid EventId { get; init; }
    public string TipoEvento { get; init; } = string.Empty;
    public DateTime OccurredAt { get; init; }
    public object? Dados { get; init; }
    public int Version { get; init; }
}

/// <summary>
/// DTO para representar o histórico completo de uma venda
/// </summary>
public record VendaHistoryDto
{
    public Guid VendaId { get; init; }
    public List<EventoDto> Eventos { get; init; } = new();
    public int TotalEventos { get; init; }
}
