using Lab08.Domain.Entities;
using Lab08.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lab08.Infrastructure.Data.Repositories;

/// <summary>
/// Repositório para Produto
/// </summary>
public class ProdutoRepository : IProdutoRepository
{
    private readonly DataContext _context;

    public ProdutoRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<Produto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Produto?> GetByIdWithCategoriaAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Produto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .OrderBy(p => p.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Produto>> GetByCategoriaAsync(int categoriaId, CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .Where(p => p.CategoriaId == categoriaId)
            .OrderBy(p => p.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Produto>> GetAtivosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .Where(p => p.Ativo)
            .OrderBy(p => p.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Produto>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .Where(p => ids.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExisteNomeAsync(string nome, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Produtos.Where(p => p.Nome == nome);
        
        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
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

    public Task DeleteAsync(Produto produto, CancellationToken cancellationToken = default)
    {
        _context.Produtos.Remove(produto);
        return Task.CompletedTask;
    }
}
