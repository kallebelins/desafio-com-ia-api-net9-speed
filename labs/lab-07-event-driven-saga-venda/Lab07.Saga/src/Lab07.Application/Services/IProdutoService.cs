using Lab07.Core.ValueObjects;

namespace Lab07.Application.Services;

/// <summary>
/// Serviço de gestão de produtos
/// </summary>
public interface IProdutoService
{
    Task<ProdutoDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProdutoDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProdutoDto> CreateAsync(string nome, string descricao, decimal preco, int estoque, CancellationToken cancellationToken = default);
    Task<ProdutoDto?> UpdateAsync(Guid id, string nome, string descricao, decimal preco, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> AtivarAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> DesativarAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ReservarEstoqueAsync(Guid produtoId, int quantidade, CancellationToken cancellationToken = default);
    Task<bool> LiberarReservaAsync(Guid produtoId, int quantidade, CancellationToken cancellationToken = default);
    Task<bool> ConfirmarReservaAsync(Guid produtoId, int quantidade, CancellationToken cancellationToken = default);
    Task<bool> AdicionarEstoqueAsync(Guid produtoId, int quantidade, CancellationToken cancellationToken = default);
}
