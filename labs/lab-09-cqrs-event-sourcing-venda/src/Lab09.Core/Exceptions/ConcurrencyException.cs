namespace Lab09.Core.Exceptions;

/// <summary>
/// Exceção para erros de concorrência no Event Store
/// </summary>
public class ConcurrencyException : Exception
{
    public Guid AggregateId { get; }
    public int ExpectedVersion { get; }
    public int ActualVersion { get; }

    public ConcurrencyException(Guid aggregateId, int expectedVersion, int actualVersion)
        : base($"Conflito de concorrência para o Aggregate {aggregateId}. " +
               $"Versão esperada: {expectedVersion}, Versão atual: {actualVersion}")
    {
        AggregateId = aggregateId;
        ExpectedVersion = expectedVersion;
        ActualVersion = actualVersion;
    }
}
