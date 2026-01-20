using Lab08.Domain.Entities;
using Lab08.Domain.ValueObjects;

namespace Lab08.Domain.Interfaces;

/// <summary>
/// Interface de repositório para Cliente
/// </summary>
public interface IClienteRepository
{
    Task<Cliente?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Cliente?> GetByCpfAsync(Cpf cpf, CancellationToken cancellationToken = default);
    Task<Cliente?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Cliente>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Cliente>> GetAtivosAsync(CancellationToken cancellationToken = default);
    Task<bool> ExisteCpfAsync(Cpf cpf, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> ExisteEmailAsync(Email email, int? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task UpdateAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task DeleteAsync(Cliente cliente, CancellationToken cancellationToken = default);
}
