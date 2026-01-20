using Lab08.Domain.Interfaces;
using Lab08.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Lab08.Infrastructure.Data;

/// <summary>
/// Unit of Work para gerenciamento de transações
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    private IClienteRepository? _clientes;
    private ICategoriaRepository? _categorias;
    private IProdutoRepository? _produtos;
    private IVendaRepository? _vendas;

    public UnitOfWork(DataContext context)
    {
        _context = context;
    }

    public IClienteRepository Clientes => _clientes ??= new ClienteRepository(_context);
    public ICategoriaRepository Categorias => _categorias ??= new CategoriaRepository(_context);
    public IProdutoRepository Produtos => _produtos ??= new ProdutoRepository(_context);
    public IVendaRepository Vendas => _vendas ??= new VendaRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
        _disposed = true;
    }
}
