namespace Lab07.Core.ValueObjects;

/// <summary>
/// Resultado de uma execução de saga
/// </summary>
public class SagaResult
{
    public Guid SagaId { get; init; }
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public object? Data { get; init; }

    public static SagaResult Success(Guid sagaId, object? data = null) => new()
    {
        SagaId = sagaId,
        IsSuccess = true,
        Data = data
    };

    public static SagaResult Failed(Guid sagaId, string errorMessage) => new()
    {
        SagaId = sagaId,
        IsSuccess = false,
        ErrorMessage = errorMessage
    };
}

/// <summary>
/// Resultado tipado de uma execução de saga
/// </summary>
public class SagaResult<T> : SagaResult
{
    public new T? Data { get; init; }

    public static SagaResult<T> Success(Guid sagaId, T? data) => new()
    {
        SagaId = sagaId,
        IsSuccess = true,
        Data = data
    };

    public new static SagaResult<T> Failed(Guid sagaId, string errorMessage) => new()
    {
        SagaId = sagaId,
        IsSuccess = false,
        ErrorMessage = errorMessage
    };
}
