using Lab07.Application.Services;
using Microsoft.Extensions.Logging;

namespace Lab07.Application.Sagas.Steps;

/// <summary>
/// Step 5: Notificar o cliente sobre a venda (via email)
/// </summary>
public class NotificarClienteStep : ISagaStep<CriarVendaSagaContext>
{
    private readonly INotificacaoService _notificacaoService;
    private readonly ILogger<NotificarClienteStep> _logger;

    public string Name => "NotificarCliente";
    public int Order => 5;
    public bool CanCompensate => false; // Email já enviado não pode ser "des-enviado"

    public NotificarClienteStep(
        INotificacaoService notificacaoService,
        ILogger<NotificarClienteStep> logger)
    {
        _notificacaoService = notificacaoService;
        _logger = logger;
    }

    public async Task ExecuteAsync(CriarVendaSagaContext data, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(data.ClienteEmail))
        {
            _logger.LogWarning(
                "Cliente {ClienteId} não tem email cadastrado",
                data.ClienteId);
            data.NotificacaoEnviada = false;
            return;
        }

        _logger.LogInformation(
            "Notificando cliente {ClienteId} sobre venda {VendaId}",
            data.ClienteId, data.VendaId);

        try
        {
            await _notificacaoService.EnviarConfirmacaoVendaAsync(
                data.ClienteEmail,
                data.ClienteNome ?? "Cliente",
                data.VendaId,
                data.ValorTotal,
                data.ProdutosValidados.Count,
                cancellationToken);

            data.NotificacaoEnviada = true;

            _logger.LogInformation(
                "Notificação enviada para {Email}",
                data.ClienteEmail);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Falha ao enviar notificação para {Email}. Continuando sem erro.",
                data.ClienteEmail);
            
            // Não marca como erro - notificação é best-effort
            data.NotificacaoEnviada = false;
        }
    }

    public Task CompensateAsync(CriarVendaSagaContext data, CancellationToken cancellationToken = default)
    {
        // Email já enviado não pode ser compensado
        // Poderia enviar um email de cancelamento, mas isso seria um novo step
        _logger.LogInformation(
            "Step NotificarCliente não requer compensação");
        return Task.CompletedTask;
    }
}
