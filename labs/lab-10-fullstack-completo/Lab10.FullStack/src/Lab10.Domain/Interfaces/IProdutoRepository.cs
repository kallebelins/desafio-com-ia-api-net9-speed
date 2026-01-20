using Lab10.Domain.Entities;

namespace Lab10.Domain.Interfaces;

/// <summary>
/// Interface do repositório de Produto
/// </summary>
public interface IProdutoRepository
{
    Task<Produto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Produto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Produto>> GetByCategoriaAsync(int categoriaId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Produto>> GetAtivosAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Produto>> GetComEstoqueAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Produto produto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Produto produto, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default);
}
