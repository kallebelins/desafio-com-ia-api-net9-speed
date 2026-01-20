namespace Lab10.Application.Sagas;

/// <summary>
/// Interface para steps de uma Saga
/// </summary>
public interface ISagaStep<TContext> where TContext : class
{
    /// <summary>
    /// Nome do step para logs e auditoria
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Ordem de execução do step
    /// </summary>
    int Order { get; }

    /// <summary>
    /// Indica se este step pode ser compensado
    /// </summary>
    bool CanCompensate { get; }

    /// <summary>
    /// Executa a ação principal do step
    /// </summary>
    Task ExecuteAsync(TContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executa a compensação (rollback) do step
    /// </summary>
    Task CompensateAsync(TContext context, CancellationToken cancellationToken = default);
}
