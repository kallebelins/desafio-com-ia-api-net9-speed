using Lab07.Application.Services;
using Microsoft.Extensions.Logging;

namespace Lab07.Application.Sagas.Steps;

/// <summary>
/// Step 1: Validar se o cliente existe e está ativo
/// </summary>
public class ValidarClienteStep : ISagaStep<CriarVendaSagaContext>
{
    private readonly IClienteService _clienteService;
    private readonly ILogger<ValidarClienteStep> _logger;

    public string Name => "ValidarCliente";
    public int Order => 1;
    public bool CanCompensate => false; // Apenas validação, nada a compensar

    public ValidarClienteStep(
        IClienteService clienteService,
        ILogger<ValidarClienteStep> logger)
    {
        _clienteService = clienteService;
        _logger = logger;
    }

    public async Task ExecuteAsync(CriarVendaSagaContext data, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Validando cliente {ClienteId}",
            data.ClienteId);

        var cliente = await _clienteService.GetByIdAsync(data.ClienteId, cancellationToken);

        if (cliente == null)
        {
            data.SetError($"Cliente {data.ClienteId} não encontrado");
            return;
        }

        if (!cliente.Ativo)
        {
            data.SetError($"Cliente {data.ClienteId} não está ativo");
            return;
        }

        // Guarda dados do cliente para uso posterior
        data.ClienteNome = cliente.Nome;
        data.ClienteEmail = cliente.Email;

        _logger.LogInformation(
            "Cliente {ClienteId} ({Nome}) validado com sucesso",
            data.ClienteId, cliente.Nome);
    }

    public Task CompensateAsync(CriarVendaSagaContext data, CancellationToken cancellationToken = default)
    {
        // Nada a compensar - apenas validação
        return Task.CompletedTask;
    }
}
