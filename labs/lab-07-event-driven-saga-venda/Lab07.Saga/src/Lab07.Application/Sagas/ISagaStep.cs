namespace Lab07.Application.Sagas;

/// <summary>
/// Interface para um step de saga
/// </summary>
/// <typeparam name="TData">Tipo do contexto de dados da saga</typeparam>
public interface ISagaStep<TData> where TData : class
{
    /// <summary>
    /// Nome do step
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Ordem de execução
    /// </summary>
    int Order { get; }

    /// <summary>
    /// Indica se o step pode ser compensado
    /// </summary>
    bool CanCompensate { get; }

    /// <summary>
    /// Executa o step
    /// </summary>
    Task ExecuteAsync(TData data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Compensa (reverte) o step
    /// </summary>
    Task CompensateAsync(TData data, CancellationToken cancellationToken = default);
}
