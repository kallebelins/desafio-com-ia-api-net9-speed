using Microsoft.Extensions.Logging;
using Lab10.Application.Interfaces;

namespace Lab10.Infrastructure.Email;

/// <summary>
/// Implementação simulada do serviço de email
/// </summary>
public class SmtpEmailService : IEmailService
{
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(ILogger<SmtpEmailService> logger)
    {
        _logger = logger;
    }

    public async Task EnviarEmailBoasVindasAsync(string email, string nome, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[EMAIL] Enviando email de boas-vindas para {Email} - Nome: {Nome}",
            email, nome);

        // Simular envio
        await Task.Delay(100, cancellationToken);

        _logger.LogInformation("[EMAIL] Email de boas-vindas enviado com sucesso");
    }

    public async Task EnviarEmailConfirmacaoVendaAsync(
        string email, 
        string nomeCliente, 
        int vendaId, 
        decimal valorTotal, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[EMAIL] Enviando confirmação de venda para {Email} - Venda #{VendaId} - Valor: {ValorTotal:C}",
            email, vendaId, valorTotal);

        // Simular envio
        await Task.Delay(100, cancellationToken);

        _logger.LogInformation("[EMAIL] Email de confirmação de venda enviado com sucesso");
    }
}
