namespace Lab10.Application.Sagas;

/// <summary>
/// Resultado da execução de uma Saga
/// </summary>
public record SagaResult
{
    public bool Sucesso { get; init; }
    public string? Erro { get; init; }
    public List<string> StepsExecutados { get; init; } = new();
    public List<string> StepsCompensados { get; init; } = new();

    public static SagaResult Success(List<string>? steps = null) => new()
    {
        Sucesso = true,
        StepsExecutados = steps ?? new()
    };

    public static SagaResult Failed(string erro, List<string>? stepsExecutados = null, List<string>? stepsCompensados = null) => new()
    {
        Sucesso = false,
        Erro = erro,
        StepsExecutados = stepsExecutados ?? new(),
        StepsCompensados = stepsCompensados ?? new()
    };
}
