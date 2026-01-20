using Microsoft.EntityFrameworkCore;
using Lab10.Domain.Entities;
using Lab10.Domain.Interfaces;

namespace Lab10.Infrastructure.Data.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly WriteDbContext _context;

    public ProdutoRepository(WriteDbContext context)
    {
        _context = context;
    }

    public async Task<Produto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Produto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Produto>> GetByCategoriaAsync(int categoriaId, CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .Where(p => p.CategoriaId == categoriaId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Produto>> GetAtivosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .Where(p => p.Ativo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Produto>> GetComEstoqueAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .Where(p => p.Ativo && (p.EstoqueAtual - p.EstoqueReservado) > 0)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Produto produto, CancellationToken cancellationToken = default)
    {
        await _context.Produtos.AddAsync(produto, cancellationToken);
    }

    public Task UpdateAsync(Produto produto, CancellationToken cancellationToken = default)
    {
        _context.Produtos.Update(produto);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Produtos.AnyAsync(p => p.Id == id, cancellationToken);
    }
}
