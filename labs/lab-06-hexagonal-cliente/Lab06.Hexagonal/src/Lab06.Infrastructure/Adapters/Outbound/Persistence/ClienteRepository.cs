using Lab06.Application.Ports.Outbound;
using Lab06.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lab06.Infrastructure.Adapters.Outbound.Persistence;

/// <summary>
/// Outbound Adapter - Implementação do repositório de clientes usando EF Core
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

    public async Task<Cliente?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var emailLower = email.ToLowerInvariant().Trim();
        // Carrega todos os clientes e filtra em memória pois o EF Core não traduz Value Objects
        var clientes = await _context.Clientes.ToListAsync(cancellationToken);
        return clientes.FirstOrDefault(c => c.Email.Value == emailLower);
    }

    public async Task<Cliente?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default)
    {
        var cpfNumbers = new string(cpf.Where(char.IsDigit).ToArray());
        // Carrega todos os clientes e filtra em memória pois o EF Core não traduz Value Objects
        var clientes = await _context.Clientes.ToListAsync(cancellationToken);
        return clientes.FirstOrDefault(c => c.Cpf.Value == cpfNumbers);
    }

    public async Task<IList<Cliente>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .OrderBy(c => c.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<IList<Cliente>> GetAllAtivosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .Where(c => c.Ativo)
            .OrderBy(c => c.Nome)
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

    public Task DeleteAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        _context.Clientes.Remove(cliente);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
