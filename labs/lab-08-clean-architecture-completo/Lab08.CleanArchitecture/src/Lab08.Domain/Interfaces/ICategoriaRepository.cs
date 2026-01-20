using Lab08.Domain.Entities;

namespace Lab08.Domain.Interfaces;

/// <summary>
/// Interface de repositório para Categoria
/// </summary>
public interface ICategoriaRepository
{
    Task<Categoria?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Categoria?> GetByNomeAsync(string nome, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Categoria>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Categoria>> GetAtivasAsync(CancellationToken cancellationToken = default);
    Task<bool> ExisteNomeAsync(string nome, int? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Categoria categoria, CancellationToken cancellationToken = default);
    Task UpdateAsync(Categoria categoria, CancellationToken cancellationToken = default);
    Task DeleteAsync(Categoria categoria, CancellationToken cancellationToken = default);
}
