using Lab10.Application.Interfaces;

namespace Lab10.Infrastructure.Data;

/// <summary>
/// Implementação do Unit of Work
/// </summary>
public class UnitOfWork : IUnitOfWorkApplication
{
    private readonly WriteDbContext _context;

    public UnitOfWork(WriteDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await _context.Database.RollbackTransactionAsync(cancellationToken);
    }
}
