namespace Lab08.Domain.Exceptions;

/// <summary>
/// Exceção de domínio para regras de negócio violadas
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Exceção para entidade não encontrada
/// </summary>
public class EntityNotFoundException : DomainException
{
    public string EntityName { get; }
    public object? EntityId { get; }

    public EntityNotFoundException(string entityName, object? id = null)
        : base($"{entityName} não encontrado(a)" + (id != null ? $" com ID: {id}" : ""))
    {
        EntityName = entityName;
        EntityId = id;
    }
}
