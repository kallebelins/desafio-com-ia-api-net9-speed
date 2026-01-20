using Lab08.Domain.Entities;
using Lab08.Domain.Enums;
using Lab08.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lab08.Infrastructure.Data.Repositories;

/// <summary>
/// Repositório para Venda
/// </summary>
public class VendaRepository : IVendaRepository
{
    private readonly DataContext _context;

    public VendaRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<Venda?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Vendas
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<Venda?> GetByIdWithItensAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Vendas
            .Include(v => v.Itens)
                .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<Venda?> GetByIdCompletoAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Itens)
                .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Venda>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Itens)
            .OrderByDescending(v => v.DataVenda)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Venda>> GetByClienteAsync(int clienteId, CancellationToken cancellationToken = default)
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Itens)
            .Where(v => v.ClienteId == clienteId)
            .OrderByDescending(v => v.DataVenda)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Venda>> GetByStatusAsync(StatusVenda status, CancellationToken cancellationToken = default)
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Itens)
            .Where(v => v.Status == status)
            .OrderByDescending(v => v.DataVenda)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Venda>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim, CancellationToken cancellationToken = default)
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .Where(v => v.DataVenda >= dataInicio && v.DataVenda <= dataFim)
            .OrderByDescending(v => v.DataVenda)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Venda>> GetByPeriodoComItensAsync(DateTime dataInicio, DateTime dataFim, CancellationToken cancellationToken = default)
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Itens)
                .ThenInclude(i => i.Produto)
            .Where(v => v.DataVenda >= dataInicio && v.DataVenda <= dataFim)
            .OrderByDescending(v => v.DataVenda)
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
}
