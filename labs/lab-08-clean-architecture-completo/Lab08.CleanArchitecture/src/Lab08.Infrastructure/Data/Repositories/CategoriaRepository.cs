using Lab08.Domain.Entities;
using Lab08.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lab08.Infrastructure.Data.Repositories;

/// <summary>
/// Repositório para Categoria
/// </summary>
public class CategoriaRepository : ICategoriaRepository
{
    private readonly DataContext _context;

    public CategoriaRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<Categoria?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Categorias
            .Include(c => c.Produtos)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Categoria?> GetByNomeAsync(string nome, CancellationToken cancellationToken = default)
    {
        return await _context.Categorias
            .FirstOrDefaultAsync(c => c.Nome == nome, cancellationToken);
    }

    public async Task<IReadOnlyList<Categoria>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categorias
            .Include(c => c.Produtos)
            .OrderBy(c => c.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Categoria>> GetAtivasAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categorias
            .Include(c => c.Produtos)
            .Where(c => c.Ativo)
            .OrderBy(c => c.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExisteNomeAsync(string nome, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Categorias.Where(c => c.Nome == nome);
        
        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(Categoria categoria, CancellationToken cancellationToken = default)
    {
        await _context.Categorias.AddAsync(categoria, cancellationToken);
    }

    public Task UpdateAsync(Categoria categoria, CancellationToken cancellationToken = default)
    {
        _context.Categorias.Update(categoria);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Categoria categoria, CancellationToken cancellationToken = default)
    {
        _context.Categorias.Remove(categoria);
        return Task.CompletedTask;
    }
}
