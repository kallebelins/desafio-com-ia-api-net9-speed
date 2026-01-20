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
/// Implementação do Use Case de criação de cliente
/// </summary>
public class CreateClienteUseCase : ICreateClienteUseCase
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IEmailService _emailService;
    private readonly ICpfValidationService _cpfValidationService;
    private readonly IValidator<CreateClienteRequest> _validator;

    public CreateClienteUseCase(
        IClienteRepository clienteRepository,
        IEmailService emailService,
        ICpfValidationService cpfValidationService,
        IValidator<CreateClienteRequest> validator)
    {
        _clienteRepository = clienteRepository;
        _emailService = emailService;
        _cpfValidationService = cpfValidationService;
        _validator = validator;
    }

    public async Task<IBusinessResult<ClienteResponse>> ExecuteAsync(
        CreateClienteRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Validar request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return CreateErrorResult<ClienteResponse>(string.Join("; ", errors));
        }

        // 2. Validar CPF com serviço externo
        var cpfValido = await _cpfValidationService.ValidateAsync(request.Cpf, cancellationToken);
        if (!cpfValido)
        {
            return CreateErrorResult<ClienteResponse>("CPF inválido no serviço de validação externa");
        }

        // 3. Verificar se email já existe
        var existingByEmail = await _clienteRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingByEmail != null)
        {
            return CreateErrorResult<ClienteResponse>("Email já cadastrado");
        }

        // 4. Verificar se CPF já existe
        var existingByCpf = await _clienteRepository.GetByCpfAsync(request.Cpf, cancellationToken);
        if (existingByCpf != null)
        {
            return CreateErrorResult<ClienteResponse>("CPF já cadastrado");
        }

        // 5. Criar entidade de domínio
        var cliente = new Cliente(request.Nome, request.Email, request.Cpf, request.Telefone);

        // 6. Adicionar endereço se fornecido
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
        await _clienteRepository.AddAsync(cliente, cancellationToken);
        await _clienteRepository.SaveChangesAsync(cancellationToken);

        // 8. Enviar email de boas-vindas (fire and forget)
        _ = _emailService.SendWelcomeEmailAsync(cliente.Email.Value, cliente.Nome, cancellationToken);

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
