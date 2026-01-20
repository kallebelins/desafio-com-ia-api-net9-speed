using Lab06.Application.Ports.Outbound;
using Microsoft.Extensions.Logging;

namespace Lab06.Infrastructure.Adapters.Outbound.Email;

/// <summary>
/// Outbound Adapter - Implementação do serviço de email
/// Em produção, usaria SMTP real ou serviço como SendGrid, AWS SES, etc.
/// </summary>
public class SmtpEmailService : IEmailService
{
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(ILogger<SmtpEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendWelcomeEmailAsync(string to, string customerName, CancellationToken cancellationToken = default)
    {
        // Em produção, implementar envio real de email
        _logger.LogInformation(
            "📧 [EMAIL SIMULADO] Enviando email de boas-vindas para {Email}. " +
            "Assunto: Bem-vindo(a), {CustomerName}!",
            to, customerName);

        return Task.CompletedTask;
    }

    public Task SendUpdateNotificationAsync(string to, string customerName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "📧 [EMAIL SIMULADO] Enviando notificação de atualização para {Email}. " +
            "Assunto: Seus dados foram atualizados, {CustomerName}",
            to, customerName);

        return Task.CompletedTask;
    }

    public Task SendDeactivationEmailAsync(string to, string customerName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "📧 [EMAIL SIMULADO] Enviando email de desativação para {Email}. " +
            "Assunto: Conta desativada, {CustomerName}",
            to, customerName);

        return Task.CompletedTask;
    }
}
