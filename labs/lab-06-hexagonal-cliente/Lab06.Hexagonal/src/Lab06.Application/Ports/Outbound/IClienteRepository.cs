using Lab06.Domain.Entities;

namespace Lab06.Application.Ports.Outbound;

/// <summary>
/// Outbound Port (Driven) - Interface do repositório de clientes
/// </summary>
public interface IClienteRepository
{
    Task<Cliente?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Cliente?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Cliente?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default);
    Task<IList<Cliente>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IList<Cliente>> GetAllAtivosAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task UpdateAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task DeleteAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
