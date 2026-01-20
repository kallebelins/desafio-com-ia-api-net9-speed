using Lab10.Domain.Entities;
using Lab10.Domain.ValueObjects;

namespace Lab10.Domain.Interfaces;

/// <summary>
/// Interface do repositório de Cliente
/// </summary>
public interface IClienteRepository
{
    Task<Cliente?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Cliente?> GetByCpfAsync(CPF cpf, CancellationToken cancellationToken = default);
    Task<Cliente?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Cliente>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Cliente>> GetAtivosAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task UpdateAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCpfAsync(CPF cpf, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
}
