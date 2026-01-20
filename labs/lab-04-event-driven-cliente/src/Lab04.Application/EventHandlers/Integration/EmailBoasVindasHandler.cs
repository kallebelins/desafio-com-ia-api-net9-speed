using Lab04.Core.Contract.Events;
using Lab04.Core.Events.Integration;
using Microsoft.Extensions.Logging;

namespace Lab04.Application.EventHandlers.Integration;

/// <summary>
/// Handler do evento de integração para envio de email de boas-vindas
/// Este handler é executado pelo consumer do RabbitMQ
/// </summary>
public class EmailBoasVindasHandler : IIntegrationEventHandler<ClienteCriadoIntegrationEvent>
{
    private readonly ILogger<EmailBoasVindasHandler> _logger;

    public EmailBoasVindasHandler(ILogger<EmailBoasVindasHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(ClienteCriadoIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Sending welcome email to {Nome} ({Email}) - ClienteId: {ClienteId}",
            integrationEvent.Nome,
            integrationEvent.Email,
            integrationEvent.ClienteId);

        // Simular envio de email de boas-vindas
        // Em produção, aqui seria integração com serviço de email (SendGrid, SMTP, etc.)
        _logger.LogInformation(
            "============================================");
        _logger.LogInformation(
            "📧 EMAIL DE BOAS-VINDAS ENVIADO!");
        _logger.LogInformation(
            "   Para: {Email}", integrationEvent.Email);
        _logger.LogInformation(
            "   Nome: {Nome}", integrationEvent.Nome);
        _logger.LogInformation(
            "   Mensagem: Bem-vindo(a) ao nosso sistema!");
        _logger.LogInformation(
            "============================================");

        return Task.CompletedTask;
    }
}
