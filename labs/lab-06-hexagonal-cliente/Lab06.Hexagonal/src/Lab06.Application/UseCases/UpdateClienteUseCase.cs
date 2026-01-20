using FluentValidation;
using Lab06.Application.DTOs.Requests;
using Lab06.Application.DTOs.Responses;
using Lab06.Application.Ports.Inbound;
using Lab06.Application.Ports.Outbound;
using Lab06.Domain.Entities;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Core.ValueObjects.Logic;

namespace Lab06.Application.UseCases;

/// <summary>
/// Implementação do Use Case de atualização de cliente
/// </summary>
public class UpdateClienteUseCase : IUpdateClienteUseCase
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IEmailService _emailService;
    private readonly IValidator<UpdateClienteRequest> _validator;

    public UpdateClienteUseCase(
        IClienteRepository clienteRepository,
        IEmailService emailService,
        IValidator<UpdateClienteRequest> validator)
    {
        _clienteRepository = clienteRepository;
        _emailService = emailService;
        _validator = validator;
    }

    public async Task<IBusinessResult<ClienteResponse>> ExecuteAsync(
        int id,
        UpdateClienteRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Validar request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return CreateErrorResult<ClienteResponse>(string.Join("; ", errors));
        }

        // 2. Buscar cliente existente
        var cliente = await _clienteRepository.GetByIdAsync(id, cancellationToken);
        if (cliente == null)
        {
            return CreateErrorResult<ClienteResponse>("Cliente não encontrado");
        }

        // 3. Verificar se email já existe em outro cliente
        var existingByEmail = await _clienteRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingByEmail != null && existingByEmail.Id != id)
        {
            return CreateErrorResult<ClienteResponse>("Email já cadastrado para outro cliente");
        }

        // 4. Atualizar dados
        cliente.SetNome(request.Nome);
        cliente.SetEmail(request.Email);
        cliente.SetTelefone(request.Telefone);

        // 5. Atualizar status se fornecido
        if (request.Ativo.HasValue)
        {
            if (request.Ativo.Value)
                cliente.Ativar();
            else
                cliente.Desativar();
        }

        // 6. Atualizar endereço
        if (request.Endereco != null)
        {
            cliente.SetEndereco(
                request.Endereco.Logradouro,
                request.Endereco.Numero,
                request.Endereco.Complemento,
                request.Endereco.Bairro,
                request.Endereco.Cidade,
                request.Endereco.Estado,
                request.Endereco.CEP
            );
        }

        // 7. Persistir
        await _clienteRepository.UpdateAsync(cliente, cancellationToken);
        await _clienteRepository.SaveChangesAsync(cancellationToken);

        // 8. Enviar notificação de atualização (fire and forget)
        _ = _emailService.SendUpdateNotificationAsync(cliente.Email.Value, cliente.Nome, cancellationToken);

        // 9. Retornar response
        var response = MapToResponse(cliente);
        return new BusinessResult<ClienteResponse>(response);
    }

    private static IBusinessResult<T> CreateErrorResult<T>(string message)
    {
        IReadOnlyCollection<IMessageResult> messages = new List<IMessageResult>
        {
            new MessageResult(message, MessageType.Error)
        };
        return new BusinessResult<T>(default!, messages);
    }

    private static ClienteResponse MapToResponse(Cliente cliente)
    {
        EnderecoResponse? enderecoResponse = null;
        if (cliente.Endereco != null)
        {
            enderecoResponse = new EnderecoResponse(
                cliente.Endereco.Logradouro,
                cliente.Endereco.Numero,
                cliente.Endereco.Complemento,
                cliente.Endereco.Bairro,
                cliente.Endereco.Cidade,
                cliente.Endereco.Estado,
                cliente.Endereco.CEP,
                cliente.Endereco.CEPFormatado,
                cliente.Endereco.EnderecoCompleto
            );
        }

        return new ClienteResponse(
            cliente.Id,
            cliente.Nome,
            cliente.Email.Value,
            cliente.Cpf.Value,
            cliente.Cpf.Formatted,
            cliente.Telefone,
            cliente.Ativo,
            cliente.DataCriacao,
            cliente.DataAtualizacao,
            enderecoResponse
        );
    }
}
