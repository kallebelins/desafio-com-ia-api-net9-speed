using Lab08.Domain.Entities;
using Lab08.Domain.Interfaces;
using Lab08.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Lab08.Infrastructure.Data.Repositories;

/// <summary>
/// Repositório para Cliente
/// </summary>
public class ClienteRepository : IClienteRepository
{
    private readonly DataContext _context;

    public ClienteRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<Cliente?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Cliente?> GetByCpfAsync(Cpf cpf, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.Cpf.Valor == cpf.Valor, cancellationToken);
    }

    public async Task<Cliente?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.Email.Valor == email.Valor, cancellationToken);
    }

    public async Task<IReadOnlyList<Cliente>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .OrderBy(c => c.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Cliente>> GetAtivosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .Where(c => c.Ativo)
            .OrderBy(c => c.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExisteCpfAsync(Cpf cpf, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Clientes.Where(c => c.Cpf.Valor == cpf.Valor);
        
        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> ExisteEmailAsync(Email email, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Clientes.Where(c => c.Email.Valor == email.Valor);
        
        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        await _context.Clientes.AddAsync(cliente, cancellationToken);
    }

    public Task UpdateAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        _context.Clientes.Update(cliente);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        _context.Clientes.Remove(cliente);
        return Task.CompletedTask;
    }
}
