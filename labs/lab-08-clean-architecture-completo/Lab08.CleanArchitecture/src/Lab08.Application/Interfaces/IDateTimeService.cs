namespace Lab08.Application.Interfaces;

/// <summary>
/// Interface para serviço de data/hora (testabilidade)
/// </summary>
public interface IDateTimeService
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}
