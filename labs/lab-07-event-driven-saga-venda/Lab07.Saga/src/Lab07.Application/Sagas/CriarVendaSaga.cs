using Lab07.Application.Sagas.Steps;
using Lab07.Core.Enums;
using Lab07.Core.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Lab07.Application.Sagas;

/// <summary>
/// Saga para criação de venda - orquestra todo o processo
/// </summary>
public class CriarVendaSaga
{
    private readonly IEnumerable<ISagaStep<CriarVendaSagaContext>> _steps;
    private readonly ILogger<CriarVendaSaga> _logger;

    public Guid SagaId { get; } = Guid.NewGuid();
    public SagaStatus Status { get; private set; } = SagaStatus.NaoIniciada;
    public int CurrentStep { get; private set; }

    public CriarVendaSaga(
        IEnumerable<ISagaStep<CriarVendaSagaContext>> steps,
        ILogger<CriarVendaSaga> logger)
    {
        _steps = steps.OrderBy(s => s.Order).ToList();
        _logger = logger;
    }

    /// <summary>
    /// Executa a saga de criação de venda
    /// </summary>
    public async Task<SagaResult<VendaDto>> ExecuteAsync(
        CriarVendaRequest request,
        CancellationToken cancellationToken = default)
    {
        var context = new CriarVendaSagaContext
        {
            VendaId = Guid.NewGuid(),
            ClienteId = request.ClienteId,
            Itens = request.Itens
        };

        var executedSteps = new Stack<ISagaStep<CriarVendaSagaContext>>();

        _logger.LogInformation(
            "Iniciando saga {SagaId} para venda {VendaId}",
            SagaId, context.VendaId);

        Status = SagaStatus.Executando;

        try
        {
            foreach (var step in _steps)
            {
                _logger.LogInformation(
                    "Saga {SagaId}: Executando step {Step}",
                    SagaId, step.Name);

                await step.ExecuteAsync(context, cancellationToken);
                executedSteps.Push(step);
                CurrentStep++;

                if (context.Failed)
                {
                    throw new SagaStepException(step.Name, context.ErrorMessage ?? "Erro desconhecido");
                }
            }

            Status = SagaStatus.Concluida;

            _logger.LogInformation(
                "Saga {SagaId} concluída com sucesso",
                SagaId);

            return SagaResult<VendaDto>.Success(SagaId, context.VendaCriada);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Saga {SagaId} falhou no step {Step}, iniciando compensação",
                SagaId, executedSteps.TryPeek(out var failedStep) ? failedStep.Name : "unknown");

            Status = SagaStatus.Compensando;
            await CompensateAsync(context, executedSteps, cancellationToken);
            Status = SagaStatus.Compensada;

            return SagaResult<VendaDto>.Failed(SagaId, ex.Message);
        }
    }

    /// <summary>
    /// Executa a compensação dos steps já executados
    /// </summary>
    private async Task CompensateAsync(
        CriarVendaSagaContext context,
        Stack<ISagaStep<CriarVendaSagaContext>> executedSteps,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Saga {SagaId}: Iniciando compensação de {Count} steps",
            SagaId, executedSteps.Count);

        while (executedSteps.TryPop(out var step))
        {
            if (!step.CanCompensate)
            {
                _logger.LogWarning(
                    "Saga {SagaId}: Step {Step} não pode ser compensado",
                    SagaId, step.Name);
                continue;
            }

            try
            {
                _logger.LogInformation(
                    "Saga {SagaId}: Compensando step {Step}",
                    SagaId, step.Name);

                await step.CompensateAsync(context, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Saga {SagaId}: Falha ao compensar step {Step}",
                    SagaId, step.Name);
                // Continua compensando os outros steps
            }
        }

        _logger.LogInformation(
            "Saga {SagaId}: Compensação concluída",
            SagaId);
    }
}

/// <summary>
/// Exceção lançada quando um step da saga falha
/// </summary>
public class SagaStepException : Exception
{
    public string StepName { get; }

    public SagaStepException(string stepName, string message)
        : base($"Step '{stepName}' falhou: {message}")
    {
        StepName = stepName;
    }
}
