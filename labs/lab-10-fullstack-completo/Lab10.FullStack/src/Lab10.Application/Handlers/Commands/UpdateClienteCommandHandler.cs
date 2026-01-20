using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Core.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;
using Microsoft.Extensions.Logging;
using Lab10.Application.Commands.Clientes;
using Lab10.Application.DTOs;
using Lab10.Application.Interfaces;
using Lab10.Domain.Interfaces;
using Lab10.Domain.ValueObjects;

namespace Lab10.Application.Handlers.Commands;

public class UpdateClienteCommandHandler : IMediatorCommandHandler<UpdateClienteCommand, IBusinessResult<ClienteDto>>
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IUnitOfWorkApplication _unitOfWork;
    private readonly ILogger<UpdateClienteCommandHandler> _logger;

    public UpdateClienteCommandHandler(
        IClienteRepository clienteRepository,
        IUnitOfWorkApplication unitOfWork,
        ILogger<UpdateClienteCommandHandler> logger)
    {
        _clienteRepository = clienteRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IBusinessResult<ClienteDto>> Handle(UpdateClienteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Atualizando cliente: {Id}", request.Id);

            var cliente = await _clienteRepository.GetByIdAsync(request.Id, cancellationToken);
            if (cliente == null)
                return CreateErrorResult<ClienteDto>("Cliente não encontrado");

            // Validar e criar Value Objects
            Lab10.Domain.ValueObjects.Email email;
            try
            {
                email = Lab10.Domain.ValueObjects.Email.Create(request.Email);
            }
            catch (Domain.Exceptions.DomainException ex)
            {
                return CreateErrorResult<ClienteDto>($"Email inválido: {ex.Message}");
            }

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

            // Atualizar entidade
            cliente.Atualizar(request.Nome, email, endereco);
            
            await _clienteRepository.UpdateAsync(cliente, cancellationToken);
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

            _logger.LogInformation("Cliente atualizado com sucesso: {Id}", cliente.Id);
            return new BusinessResult<ClienteDto>(dto);
        }
        catch (Domain.Exceptions.DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao atualizar cliente");
            return CreateErrorResult<ClienteDto>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar cliente");
            return CreateErrorResult<ClienteDto>("Erro interno ao atualizar cliente");
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
