using Microsoft.EntityFrameworkCore;
using Lab10.Domain.Entities;
using Lab10.Domain.Enums;
using Lab10.Domain.Interfaces;

namespace Lab10.Infrastructure.Data.Repositories;

public class PagamentoRepository : IPagamentoRepository
{
    private readonly WriteDbContext _context;

    public PagamentoRepository(WriteDbContext context)
    {
        _context = context;
    }

    public async Task<Pagamento?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Pagamentos.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<Pagamento?> GetByVendaIdAsync(int vendaId, CancellationToken cancellationToken = default)
    {
        return await _context.Pagamentos
            .FirstOrDefaultAsync(p => p.VendaId == vendaId, cancellationToken);
    }

    public async Task<IEnumerable<Pagamento>> GetByStatusAsync(PagamentoStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Pagamentos
            .Where(p => p.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Pagamento pagamento, CancellationToken cancellationToken = default)
    {
        await _context.Pagamentos.AddAsync(pagamento, cancellationToken);
    }

    public Task UpdateAsync(Pagamento pagamento, CancellationToken cancellationToken = default)
    {
        _context.Pagamentos.Update(pagamento);
        return Task.CompletedTask;
    }
}
