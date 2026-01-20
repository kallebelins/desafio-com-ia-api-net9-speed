namespace Lab08.Application.Interfaces;

/// <summary>
/// Interface base para Use Cases com Input e Output
/// </summary>
/// <typeparam name="TInput">Tipo do input</typeparam>
/// <typeparam name="TOutput">Tipo do output</typeparam>
public interface IUseCase<in TInput, TOutput>
{
    Task<TOutput> ExecuteAsync(TInput input, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface base para Use Cases somente com Output (sem input)
/// </summary>
/// <typeparam name="TOutput">Tipo do output</typeparam>
public interface IUseCase<TOutput>
{
    Task<TOutput> ExecuteAsync(CancellationToken cancellationToken = default);
}
