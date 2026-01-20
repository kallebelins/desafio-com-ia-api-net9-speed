using Lab07.Core.Entities;
using Lab07.Core.ValueObjects;
using Microsoft.Extensions.Logging;
using Mvp24Hours.Core.Contract.Data;

namespace Lab07.Application.Services;

/// <summary>
/// Implementação do serviço de clientes
/// </summary>
public class ClienteService : IClienteService
{
    private readonly IUnitOfWorkAsync _unitOfWork;
    private readonly ILogger<ClienteService> _logger;

    public ClienteService(
        IUnitOfWorkAsync unitOfWork,
        ILogger<ClienteService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ClienteDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Cliente>();
        var cliente = await repository.GetByIdAsync(id);
        
        if (cliente == null) return null;

        return MapToDto(cliente);
    }

    public async Task<IEnumerable<ClienteDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Cliente>();
        var clientes = await repository.ListAsync();
        
        return clientes.Where(c => c.Removed == null).Select(MapToDto);
    }

    public async Task<ClienteDto> CreateAsync(string nome, string email, string telefone, CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Cliente>();
        
        var cliente = new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = nome,
            Email = email,
            Telefone = telefone,
            Ativo = true,
            Created = DateTime.UtcNow
        };

        await repository.AddAsync(cliente);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Cliente {ClienteId} criado: {Nome}", cliente.Id, nome);

        return MapToDto(cliente);
    }

    public async Task<ClienteDto?> UpdateAsync(Guid id, string nome, string email, string telefone, CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Cliente>();
        var cliente = await repository.GetByIdAsync(id);
        
        if (cliente == null) return null;

        cliente.Nome = nome;
        cliente.Email = email;
        cliente.Telefone = telefone;
        cliente.Modified = DateTime.UtcNow;

        await repository.ModifyAsync(cliente);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Cliente {ClienteId} atualizado", id);

        return MapToDto(cliente);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Cliente>();
        var cliente = await repository.GetByIdAsync(id);
        
        if (cliente == null) return false;

        cliente.Removed = DateTime.UtcNow;
        cliente.Ativo = false;

        await repository.ModifyAsync(cliente);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Cliente {ClienteId} removido", id);

        return true;
    }

    public async Task<bool> AtivarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Cliente>();
        var cliente = await repository.GetByIdAsync(id);
        
        if (cliente == null) return false;

        cliente.Ativo = true;
        cliente.Modified = DateTime.UtcNow;

        await repository.ModifyAsync(cliente);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Cliente {ClienteId} ativado", id);

        return true;
    }

    public async Task<bool> DesativarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var repository = _unitOfWork.GetRepository<Cliente>();
        var cliente = await repository.GetByIdAsync(id);
        
        if (cliente == null) return false;

        cliente.Ativo = false;
        cliente.Modified = DateTime.UtcNow;

        await repository.ModifyAsync(cliente);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Cliente {ClienteId} desativado", id);

        return true;
    }

    private static ClienteDto MapToDto(Cliente cliente) => new()
    {
        Id = cliente.Id,
        Nome = cliente.Nome,
        Email = cliente.Email,
        Telefone = cliente.Telefone,
        Ativo = cliente.Ativo,
        Created = cliente.Created
    };
}
