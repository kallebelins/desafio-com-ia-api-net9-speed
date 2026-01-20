using Microsoft.Extensions.Logging;

namespace Lab10.Application.Sagas;

/// <summary>
/// Saga para processamento completo de uma venda
/// </summary>
public class ProcessarVendaSaga
{
    private readonly IEnumerable<ISagaStep<ProcessarVendaSagaContext>> _steps;
    private readonly ILogger<ProcessarVendaSaga> _logger;

    public ProcessarVendaSaga(
        IEnumerable<ISagaStep<ProcessarVendaSagaContext>> steps,
        ILogger<ProcessarVendaSaga> logger)
    {
        _steps = steps.OrderBy(s => s.Order).ToList();
        _logger = logger;
    }

    public async Task<SagaResult> ExecuteAsync(ProcessarVendaSagaContext context, CancellationToken cancellationToken = default)
    {
        var executedSteps = new Stack<ISagaStep<ProcessarVendaSagaContext>>();
        var stepsExecutados = new List<string>();

        _logger.LogInformation("Iniciando Saga de processamento de venda para cliente: {ClienteId}", context.ClienteId);

        try
        {
            foreach (var step in _steps)
            {
                _logger.LogInformation("Executando step: {StepName}", step.Name);

                await step.ExecuteAsync(context, cancellationToken);
                executedSteps.Push(step);
                stepsExecutados.Add(step.Name);

                _logger.LogInformation("Step concluído com sucesso: {StepName}", step.Name);
            }

            _logger.LogInformation("Saga concluída com sucesso. VendaId: {VendaId}", context.Venda?.Id);
            return SagaResult.Success(stepsExecutados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha na Saga durante step. Iniciando compensação...");

            var stepsCompensados = await CompensateAsync(context, executedSteps, cancellationToken);

            return SagaResult.Failed(ex.Message, stepsExecutados, stepsCompensados);
        }
    }

    private async Task<List<string>> CompensateAsync(
        ProcessarVendaSagaContext context,
        Stack<ISagaStep<ProcessarVendaSagaContext>> executedSteps,
        CancellationToken cancellationToken)
    {
        var stepsCompensados = new List<string>();

        _logger.LogInformation("Iniciando compensação da Saga...");

        while (executedSteps.TryPop(out var step))
        {
            if (!step.CanCompensate)
            {
                _logger.LogWarning("Step {StepName} não pode ser compensado", step.Name);
                continue;
            }

            try
            {
                _logger.LogInformation("Compensando step: {StepName}", step.Name);
                await step.CompensateAsync(context, cancellationToken);
                stepsCompensados.Add(step.Name);
                _logger.LogInformation("Step compensado com sucesso: {StepName}", step.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao compensar step: {StepName}", step.Name);
            }
        }

        _logger.LogInformation("Compensação concluída. Steps compensados: {Count}", stepsCompensados.Count);
        return stepsCompensados;
    }
}
