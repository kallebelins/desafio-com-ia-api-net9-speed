using Microsoft.EntityFrameworkCore;
using Lab10.Domain.Entities;
using Lab10.Domain.Interfaces;
using Lab10.Domain.ValueObjects;

namespace Lab10.Infrastructure.Data.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly WriteDbContext _context;

    public ClienteRepository(WriteDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<Cliente?> GetByCpfAsync(CPF cpf, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.Cpf.Valor == cpf.Valor, cancellationToken);
    }

    public async Task<Cliente?> GetByEmailAsync(Lab10.Domain.ValueObjects.Email email, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.Email.Valor == email.Valor, cancellationToken);
    }

    public async Task<IEnumerable<Cliente>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Clientes.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Cliente>> GetAtivosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .Where(c => c.Ativo)
            .ToListAsync(cancellationToken);
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

    public async Task<bool> ExistsByCpfAsync(CPF cpf, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes.AnyAsync(c => c.Cpf.Valor == cpf.Valor, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(Lab10.Domain.ValueObjects.Email email, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes.AnyAsync(c => c.Email.Valor == email.Valor, cancellationToken);
    }
}
