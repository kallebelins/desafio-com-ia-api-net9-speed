namespace Lab09.Infrastructure.EventStore;

/// <summary>
/// Entidade para armazenar eventos no banco de dados
/// </summary>
public class StoredEvent
{
    public long Id { get; set; }
    public Guid EventId { get; set; }
    public Guid AggregateId { get; set; }
    public string AggregateType { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string EventData { get; set; } = string.Empty;
    public int Version { get; set; }
    public DateTime Timestamp { get; set; }
    public string? UserId { get; set; }
    public string? Metadata { get; set; }
}
