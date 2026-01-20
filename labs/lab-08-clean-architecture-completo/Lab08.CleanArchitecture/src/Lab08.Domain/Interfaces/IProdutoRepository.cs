using Lab08.Domain.Entities;

namespace Lab08.Domain.Interfaces;

/// <summary>
/// Interface de repositório para Produto
/// </summary>
public interface IProdutoRepository
{
    Task<Produto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Produto?> GetByIdWithCategoriaAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Produto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Produto>> GetByCategoriaAsync(int categoriaId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Produto>> GetAtivosAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Produto>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<bool> ExisteNomeAsync(string nome, int? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Produto produto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Produto produto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Produto produto, CancellationToken cancellationToken = default);
}
