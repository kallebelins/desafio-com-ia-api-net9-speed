namespace Lab10.Domain.Events.Domain;

/// <summary>
/// Evento de domínio: Cliente criado
/// </summary>
public record ClienteCriadoEvent(
    int ClienteId,
    string Nome,
    string Email,
    string Cpf,
    DateTime OcorridoEm) : IDomainEvent
{
    public ClienteCriadoEvent(int clienteId, string nome, string email, string cpf)
        : this(clienteId, nome, email, cpf, DateTime.UtcNow) { }
}
