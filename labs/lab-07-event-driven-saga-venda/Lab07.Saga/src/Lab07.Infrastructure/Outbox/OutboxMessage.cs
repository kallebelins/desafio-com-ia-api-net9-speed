using Lab07.Core.Enums;
using Mvp24Hours.Core.Entities;

namespace Lab07.Infrastructure.Outbox;

/// <summary>
/// Mensagem do outbox para garantia de entrega de eventos
/// </summary>
public class OutboxMessage : EntityBase<Guid>
{
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int RetryCount { get; set; }
    public OutboxStatus Status { get; set; } = OutboxStatus.Pendente;
    public string? Error { get; set; }
}
