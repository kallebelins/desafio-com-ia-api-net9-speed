using Lab04.Core.Contract.Events;
using Lab04.Core.Contract.Services;
using Lab04.Core.Entities;
using Lab04.Core.Events.Domain;
using Lab04.Core.ValueObjects;
using Microsoft.Extensions.Logging;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Core.ValueObjects.Logic;
using Mvp24Hours.Extensions;

namespace Lab04.Application.Services;

/// <summary>
/// Serviço de clientes com disparo de eventos de domínio
/// </summary>
public class ClienteService : IClienteService
{
    private readonly IUnitOfWorkAsync _unitOfWork;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger<ClienteService> _logger;

    public ClienteService(
        IUnitOfWorkAsync unitOfWork,
        IDomainEventDispatcher eventDispatcher,
        ILogger<ClienteService> logger)
    {
        _unitOfWork = unitOfWork;
        _eventDispatcher = eventDispatcher;
        _logger = logger;
    }

    public async Task<IBusinessResult<IEnumerable<ClienteDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all clients");
        
        var repository = _unitOfWork.GetRepository<Cliente>();
        var clientes = await repository.ListAsync();
        
        var dtos = clientes.Select(ClienteDto.FromEntity);
        return dtos.ToBusiness();
    }

    public async Task<IBusinessResult<ClienteDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving client by Id: {Id}", id);
        
        var repository = _unitOfWork.GetRepository<Cliente>();
        var cliente = await repository.GetByIdAsync(id);
        
        if (cliente == null)
        {
            _logger.LogWarning("Client not found: {Id}", id);
            return CreateErrorResult<ClienteDto>("Cliente não encontrado");
        }
        
        return ClienteDto.FromEntity(cliente).ToBusiness();
    }

    public async Task<IBusinessResult<ClienteDto>> CreateAsync(ClienteCreateDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new client: {Nome}", dto.Nome);
        
        var repository = _unitOfWork.GetRepository<Cliente>();
        
        // Verificar se já existe cliente com o mesmo email
        var emailExists = await repository.GetByAnyAsync(c => c.Email == dto.Email);
        if (emailExists)
        {
            _logger.LogWarning("Email already in use: {Email}", dto.Email);
            return CreateErrorResult<ClienteDto>($"Já existe um cliente cadastrado com o email '{dto.Email}'");
        }
        
        // Verificar se já existe cliente com o mesmo CPF
        var cpfExists = await repository.GetByAnyAsync(c => c.CPF == dto.CPF);
        if (cpfExists)
        {
            _logger.LogWarning("CPF already in use: {CPF}", dto.CPF);
            return CreateErrorResult<ClienteDto>($"Já existe um cliente cadastrado com o CPF '{dto.CPF}'");
        }
        
        var cliente = new Cliente
        {
            Nome = dto.Nome,
            Email = dto.Email,
            CPF = dto.CPF,
            Telefone = dto.Telefone,
            Ativo = true,
            DataCriacao = DateTime.UtcNow
        };

        await repository.AddAsync(cliente);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Client created successfully: {Id}", cliente.Id);

        // Disparar evento de domínio
        var domainEvent = new ClienteCriadoEvent
        {
            ClienteId = cliente.Id,
            Nome = cliente.Nome,
            Email = cliente.Email,
            CPF = cliente.CPF
        };
        
        await _eventDispatcher.DispatchAsync(domainEvent, cancellationToken);

        return ClienteDto.FromEntity(cliente).ToBusiness();
    }

    public async Task<IBusinessResult<ClienteDto>> UpdateAsync(int id, ClienteUpdateDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating client: {Id}", id);
        
        var repository = _unitOfWork.GetRepository<Cliente>();
        var cliente = await repository.GetByIdAsync(id);

        if (cliente == null)
        {
            _logger.LogWarning("Client not found for update: {Id}", id);
            return CreateErrorResult<ClienteDto>("Cliente não encontrado");
        }

        // Verificar se o novo email já está em uso por outro cliente
        if (cliente.Email != dto.Email)
        {
            var emailExists = await repository.GetByAnyAsync(c => c.Email == dto.Email && c.Id != id);
            if (emailExists)
            {
                _logger.LogWarning("Email already in use by another client: {Email}", dto.Email);
                return CreateErrorResult<ClienteDto>($"Já existe outro cliente cadastrado com o email '{dto.Email}'");
            }
        }

        cliente.Nome = dto.Nome;
        cliente.Email = dto.Email;
        cliente.Telefone = dto.Telefone;
        cliente.Ativo = dto.Ativo;
        cliente.DataAtualizacao = DateTime.UtcNow;

        await repository.ModifyAsync(cliente);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Client updated successfully: {Id}", id);

        // Disparar evento de domínio
        var domainEvent = new ClienteAtualizadoEvent
        {
            ClienteId = cliente.Id,
            Nome = cliente.Nome,
            Email = cliente.Email,
            Ativo = cliente.Ativo
        };
        
        await _eventDispatcher.DispatchAsync(domainEvent, cancellationToken);

        return ClienteDto.FromEntity(cliente).ToBusiness();
    }

    public async Task<IBusinessResult<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting client: {Id}", id);
        
        var repository = _unitOfWork.GetRepository<Cliente>();
        var cliente = await repository.GetByIdAsync(id);

        if (cliente == null)
        {
            _logger.LogWarning("Client not found for deletion: {Id}", id);
            return CreateErrorResult<bool>("Cliente não encontrado");
        }

        await repository.RemoveAsync(cliente);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Client deleted successfully: {Id}", id);

        // Disparar evento de domínio
        var domainEvent = new ClienteExcluidoEvent { ClienteId = id };
        await _eventDispatcher.DispatchAsync(domainEvent, cancellationToken);

        return true.ToBusiness();
    }
    
    /// <summary>
    /// Helper method para criar resultado de erro
    /// </summary>
    private static IBusinessResult<T> CreateErrorResult<T>(string message)
    {
        IReadOnlyCollection<IMessageResult> messages = new List<IMessageResult>
        {
            new MessageResult(message, MessageType.Error)
        };
        return new BusinessResult<T>(default, messages);
    }
}
