using Mvp24Hours.Core.Entities;

namespace Lab10.Infrastructure.Data;

/// <summary>
/// Entidade para mensagens do Outbox Pattern
/// </summary>
public class OutboxMessage : EntityBase<Guid>
{
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int RetryCount { get; set; }
    public string? Error { get; set; }
    public OutboxStatus Status { get; set; }
}

public enum OutboxStatus
{
    Pending = 0,
    Published = 1,
    Failed = 2
}
