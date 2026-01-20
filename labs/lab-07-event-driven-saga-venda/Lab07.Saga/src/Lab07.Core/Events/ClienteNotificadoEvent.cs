namespace Lab07.Core.Events;

/// <summary>
/// Evento disparado quando o cliente é notificado sobre a venda
/// </summary>
public record ClienteNotificadoEvent
{
    public Guid VendaId { get; init; }
    public Guid ClienteId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string TipoNotificacao { get; init; } = string.Empty;
    public DateTime OcorridoEm { get; init; } = DateTime.UtcNow;
}
