using Lab08.Application.Interfaces;

namespace Lab08.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de data/hora
/// </summary>
public class DateTimeService : IDateTimeService
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}
