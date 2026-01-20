using Microsoft.Extensions.Logging;
using Lab10.Domain.Exceptions;

namespace Lab10.Application.Sagas.Steps;

/// <summary>
/// Step 1: Valida se o cliente existe e está ativo
/// </summary>
public class ValidarClienteStep : ISagaStep<ProcessarVendaSagaContext>
{
    private readonly ILogger<ValidarClienteStep> _logger;

    public ValidarClienteStep(ILogger<ValidarClienteStep> logger)
    {
        _logger = logger;
    }

    public string Name => "ValidarCliente";
    public int Order => 1;
    public bool CanCompensate => false; // Validação não precisa compensar

    public Task ExecuteAsync(ProcessarVendaSagaContext context, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Validando cliente: {ClienteId}", context.ClienteId);

        if (context.Cliente == null)
            throw new DomainException("Cliente não encontrado");

        if (!context.Cliente.Ativo)
            throw new DomainException("Cliente não está ativo");

        context.ClienteValidado = true;
        _logger.LogInformation("Cliente validado com sucesso: {ClienteNome}", context.Cliente.Nome);

        return Task.CompletedTask;
    }

    public Task CompensateAsync(ProcessarVendaSagaContext context, CancellationToken cancellationToken = default)
    {
        // Nada a compensar - apenas validação
        return Task.CompletedTask;
    }
}
