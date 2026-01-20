using Microsoft.EntityFrameworkCore;
using Lab10.Domain.Entities;
using Lab10.Domain.Enums;
using Lab10.Domain.Interfaces;

namespace Lab10.Infrastructure.Data.Repositories;

public class VendaRepository : IVendaRepository
{
    private readonly WriteDbContext _context;

    public VendaRepository(WriteDbContext context)
    {
        _context = context;
    }

    public async Task<Venda?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Vendas.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<Venda?> GetByIdWithItensAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Vendas
            .Include(v => v.Itens)
            .Include(v => v.Cliente)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<Venda?> GetByIdWithItensAndPagamentoAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Vendas
            .Include(v => v.Itens)
            .Include(v => v.Cliente)
            .Include(v => v.Pagamento)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Venda>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .OrderByDescending(v => v.DataCriacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Venda>> GetByClienteAsync(int clienteId, CancellationToken cancellationToken = default)
    {
        return await _context.Vendas
            .Include(v => v.Itens)
            .Where(v => v.ClienteId == clienteId)
            .OrderByDescending(v => v.DataCriacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Venda>> GetByStatusAsync(VendaStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .Where(v => v.Status == status)
            .OrderByDescending(v => v.DataCriacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Venda>> GetByPeriodoAsync(DateTime inicio, DateTime fim, CancellationToken cancellationToken = default)
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .Where(v => v.DataCriacao >= inicio && v.DataCriacao <= fim)
            .OrderByDescending(v => v.DataCriacao)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Venda venda, CancellationToken cancellationToken = default)
    {
        await _context.Vendas.AddAsync(venda, cancellationToken);
    }

    public Task UpdateAsync(Venda venda, CancellationToken cancellationToken = default)
    {
        _context.Vendas.Update(venda);
        return Task.CompletedTask;
    }

    public async Task<int> CountByStatusAsync(VendaStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Vendas.CountAsync(v => v.Status == status, cancellationToken);
    }

    public async Task<decimal> GetTotalVendasPeriodoAsync(DateTime inicio, DateTime fim, CancellationToken cancellationToken = default)
    {
        var vendas = await _context.Vendas
            .Where(v => v.DataCriacao >= inicio && v.DataCriacao <= fim && v.Status == VendaStatus.Finalizada)
            .ToListAsync(cancellationToken);

        return vendas.Sum(v => v.ValorTotal.Valor);
    }
}
