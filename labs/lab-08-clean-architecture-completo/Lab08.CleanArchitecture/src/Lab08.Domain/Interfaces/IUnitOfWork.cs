namespace Lab08.Domain.Interfaces;

/// <summary>
/// Interface para Unit of Work - gerenciamento de transações
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IClienteRepository Clientes { get; }
    ICategoriaRepository Categorias { get; }
    IProdutoRepository Produtos { get; }
    IVendaRepository Vendas { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
