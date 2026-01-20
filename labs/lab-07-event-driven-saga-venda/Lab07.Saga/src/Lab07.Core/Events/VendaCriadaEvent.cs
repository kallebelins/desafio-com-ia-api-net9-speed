namespace Lab07.Core.Events;

/// <summary>
/// Evento disparado quando uma venda é criada/confirmada com sucesso
/// </summary>
public record VendaCriadaEvent
{
    public Guid VendaId { get; init; }
    public Guid ClienteId { get; init; }
    public string ClienteEmail { get; init; } = string.Empty;
    public decimal ValorTotal { get; init; }
    public int TotalItens { get; init; }
    public DateTime OcorridoEm { get; init; } = DateTime.UtcNow;
}
