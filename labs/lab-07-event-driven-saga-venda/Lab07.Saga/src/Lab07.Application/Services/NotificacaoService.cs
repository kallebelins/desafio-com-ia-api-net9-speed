using Microsoft.Extensions.Logging;

namespace Lab07.Application.Services;

/// <summary>
/// Implementação do serviço de notificações (simulado)
/// </summary>
public class NotificacaoService : INotificacaoService
{
    private readonly ILogger<NotificacaoService> _logger;

    public NotificacaoService(ILogger<NotificacaoService> logger)
    {
        _logger = logger;
    }

    public Task EnviarConfirmacaoVendaAsync(
        string email,
        string nomeCliente,
        Guid vendaId,
        decimal valorTotal,
        int totalItens,
        CancellationToken cancellationToken = default)
    {
        // Simula envio de email
        _logger.LogInformation(
            "[EMAIL] Enviando confirmação de venda para {Email}\n" +
            "  Destinatário: {Nome}\n" +
            "  Venda: {VendaId}\n" +
            "  Valor Total: {ValorTotal:C}\n" +
            "  Total de Itens: {TotalItens}",
            email, nomeCliente, vendaId, valorTotal, totalItens);

        // Simula delay de envio
        return Task.Delay(100, cancellationToken);
    }

    public Task EnviarCancelamentoVendaAsync(
        string email,
        string nomeCliente,
        Guid vendaId,
        string motivo,
        CancellationToken cancellationToken = default)
    {
        // Simula envio de email
        _logger.LogInformation(
            "[EMAIL] Enviando notificação de cancelamento para {Email}\n" +
            "  Destinatário: {Nome}\n" +
            "  Venda: {VendaId}\n" +
            "  Motivo: {Motivo}",
            email, nomeCliente, vendaId, motivo);

        // Simula delay de envio
        return Task.Delay(100, cancellationToken);
    }
}
