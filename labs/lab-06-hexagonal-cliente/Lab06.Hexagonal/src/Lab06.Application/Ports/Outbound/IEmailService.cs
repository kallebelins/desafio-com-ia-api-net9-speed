namespace Lab06.Application.Ports.Outbound;

/// <summary>
/// Outbound Port (Driven) - Interface para serviço de email
/// </summary>
public interface IEmailService
{
    Task SendWelcomeEmailAsync(string to, string customerName, CancellationToken cancellationToken = default);
    Task SendUpdateNotificationAsync(string to, string customerName, CancellationToken cancellationToken = default);
    Task SendDeactivationEmailAsync(string to, string customerName, CancellationToken cancellationToken = default);
}
