namespace Lab09.Infrastructure.Snapshots;

/// <summary>
/// Entidade para armazenar snapshots de Venda
/// </summary>
public class VendaSnapshot
{
    public Guid Id { get; set; }
    public Guid AggregateId { get; set; }
    public int Version { get; set; }
    public string StateJson { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
