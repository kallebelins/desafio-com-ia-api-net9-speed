using Lab04.Core.Events.Base;

namespace Lab04.Core.Events.Domain;

/// <summary>
/// Evento de domínio disparado quando um cliente é criado
/// </summary>
public record ClienteCriadoEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventType => nameof(ClienteCriadoEvent);
    
    public int ClienteId { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string CPF { get; init; } = string.Empty;
}
