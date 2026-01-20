using Microsoft.EntityFrameworkCore;
using Lab10.Domain.Entities;
using Lab10.Domain.Interfaces;

namespace Lab10.Infrastructure.Data.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly WriteDbContext _context;

    public CategoriaRepository(WriteDbContext context)
    {
        _context = context;
    }

    public async Task<Categoria?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Categorias.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<Categoria?> GetByNomeAsync(string nome, CancellationToken cancellationToken = default)
    {
        return await _context.Categorias
            .FirstOrDefaultAsync(c => c.Nome == nome, cancellationToken);
    }

    public async Task<IEnumerable<Categoria>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categorias.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Categoria>> GetAtivasAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categorias
            .Where(c => c.Ativo)
            .ToListAsync(cancellationToken);
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

    public async Task<bool> ExistsByNomeAsync(string nome, CancellationToken cancellationToken = default)
    {
        return await _context.Categorias.AnyAsync(c => c.Nome == nome, cancellationToken);
    }
}
