namespace Lab10.Domain.Exceptions;

/// <summary>
/// Exceção de domínio para erros de regras de negócio
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
