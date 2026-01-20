namespace Lab06.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando um cliente não é encontrado
/// </summary>
public class ClienteNotFoundException : DomainException
{
    public int ClienteId { get; }

    public ClienteNotFoundException(int clienteId)
        : base($"Cliente com ID {clienteId} não foi encontrado")
    {
        ClienteId = clienteId;
    }
}
