using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Core.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;
using Microsoft.Extensions.Logging;
using Lab10.Application.Commands.Clientes;
using Lab10.Application.DTOs;
using Lab10.Application.Interfaces;
using Lab10.Domain.Entities;
using Lab10.Domain.Events.Integration;
using Lab10.Domain.Interfaces;
using Lab10.Domain.ValueObjects;

namespace Lab10.Application.Handlers.Commands;

public class CreateClienteCommandHandler : IMediatorCommandHandler<CreateClienteCommand, IBusinessResult<ClienteDto>>
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IUnitOfWorkApplication _unitOfWork;
    private readonly IOutboxService _outboxService;
    private readonly ILogger<CreateClienteCommandHandler> _logger;

    public CreateClienteCommandHandler(
        IClienteRepository clienteRepository,
        IUnitOfWorkApplication unitOfWork,
        IOutboxService outboxService,
        ILogger<CreateClienteCommandHandler> logger)
    {
        _clienteRepository = clienteRepository;
        _unitOfWork = unitOfWork;
        _outboxService = outboxService;
        _logger = logger;
    }

    public async Task<IBusinessResult<ClienteDto>> Handle(CreateClienteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Criando cliente com CPF: {CPF}", request.Cpf);

            // Validar e criar Value Objects
            Email email;
            CPF cpf;
            
            try
            {
                email = Email.Create(request.Email);
            }
            catch (Domain.Exceptions.DomainException ex)
            {
                return CreateErrorResult<ClienteDto>($"Email inválido: {ex.Message}");
            }

            try
            {
                cpf = CPF.Create(request.Cpf);
            }
            catch (Domain.Exceptions.DomainException ex)
            {
                return CreateErrorResult<ClienteDto>($"CPF inválido: {ex.Message}");
            }

            // Verificar se já existe cliente com mesmo CPF ou Email
            var existente = await _clienteRepository.GetByCpfAsync(cpf, cancellationToken);
            if (existente != null)
                return CreateErrorResult<ClienteDto>("Já existe um cliente com este CPF");

            existente = await _clienteRepository.GetByEmailAsync(email, cancellationToken);
            if (existente != null)
                return CreateErrorResult<ClienteDto>("Já existe um cliente com este e-mail");

            // Criar Value Object Endereco se fornecido
            Endereco? endereco = null;
            if (request.Endereco != null)
            {
                try
                {
                    endereco = Endereco.Create(
                        request.Endereco.Logradouro,
                        request.Endereco.Numero,
                        request.Endereco.Complemento,
                        request.Endereco.Bairro,
                        request.Endereco.Cidade,
                        request.Endereco.Estado,
                        request.Endereco.Cep);
                }
                catch (ArgumentException ex)
                {
                    return CreateErrorResult<ClienteDto>($"Endereço inválido: {ex.Message}");
                }
            }

            // Criar entidade
            var cliente = new Cliente(request.Nome, email, cpf, endereco);
            
            await _clienteRepository.AddAsync(cliente, cancellationToken);

            // Adicionar evento de integração ao outbox
            var integrationEvent = new ClienteCriadoIntegrationEvent(
                cliente.Id, 
                cliente.Nome, 
                cliente.Email.Valor, 
                DateTime.UtcNow);
            await _outboxService.AddMessageAsync(integrationEvent, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Criar DTO de resposta
            EnderecoDto? enderecoDto = null;
            if (cliente.Endereco != null)
            {
                enderecoDto = new EnderecoDto(
                    cliente.Endereco.Logradouro,
                    cliente.Endereco.Numero,
                    cliente.Endereco.Complemento,
                    cliente.Endereco.Bairro,
                    cliente.Endereco.Cidade,
                    cliente.Endereco.Estado,
                    cliente.Endereco.CepFormatado);
            }

            var dto = new ClienteDto(
                cliente.Id,
                cliente.Nome,
                cliente.Email.Valor,
                cliente.Cpf.Formatado,
                enderecoDto,
                cliente.Ativo,
                cliente.DataCadastro);

            _logger.LogInformation("Cliente criado com sucesso: {Id}", cliente.Id);
            return new BusinessResult<ClienteDto>(dto);
        }
        catch (Domain.Exceptions.DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao criar cliente");
            return CreateErrorResult<ClienteDto>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar cliente");
            return CreateErrorResult<ClienteDto>("Erro interno ao criar cliente");
        }
    }

    private static IBusinessResult<T> CreateErrorResult<T>(string message)
    {
        IReadOnlyCollection<IMessageResult> messages = new List<IMessageResult>
        {
            new MessageResult(message, MessageType.Error)
        };
        return new BusinessResult<T>(default!, messages);
    }
}
