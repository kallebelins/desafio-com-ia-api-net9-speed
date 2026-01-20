using Lab10.Domain.Entities;

namespace Lab10.Domain.Interfaces;

/// <summary>
/// Interface do repositório de Categoria
/// </summary>
public interface ICategoriaRepository
{
    Task<Categoria?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Categoria?> GetByNomeAsync(string nome, CancellationToken cancellationToken = default);
    Task<IEnumerable<Categoria>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Categoria>> GetAtivasAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Categoria categoria, CancellationToken cancellationToken = default);
    Task UpdateAsync(Categoria categoria, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNomeAsync(string nome, CancellationToken cancellationToken = default);
}
