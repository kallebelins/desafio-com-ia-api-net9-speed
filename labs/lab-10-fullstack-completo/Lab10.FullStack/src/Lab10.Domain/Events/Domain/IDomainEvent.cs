namespace Lab10.Domain.Events.Domain;

/// <summary>
/// Interface base para eventos de domínio
/// </summary>
public interface IDomainEvent
{
    DateTime OcorridoEm { get; }
}
