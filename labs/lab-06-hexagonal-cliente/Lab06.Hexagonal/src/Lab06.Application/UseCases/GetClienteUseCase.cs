using Lab06.Application.DTOs.Responses;
using Lab06.Application.Ports.Inbound;
using Lab06.Application.Ports.Outbound;
using Lab06.Domain.Entities;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Core.ValueObjects.Logic;

namespace Lab06.Application.UseCases;

/// <summary>
/// Implementação do Use Case de consulta de clientes
/// </summary>
public class GetClienteUseCase : IGetClienteUseCase
{
    private readonly IClienteRepository _clienteRepository;

    public GetClienteUseCase(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<IBusinessResult<ClienteResponse>> ExecuteByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id, cancellationToken);

        if (cliente == null)
        {
            return CreateErrorResult<ClienteResponse>("Cliente não encontrado");
        }

        var response = MapToResponse(cliente);
        return new BusinessResult<ClienteResponse>(response);
    }

    public async Task<IBusinessResult<ClienteListResponse>> ExecuteAllAsync(
        CancellationToken cancellationToken = default)
    {
        var clientes = await _clienteRepository.GetAllAsync(cancellationToken);

        var items = clientes.Select(MapToResponse).ToList();
        var response = new ClienteListResponse(items, items.Count);

        return new BusinessResult<ClienteListResponse>(response);
    }

    public async Task<IBusinessResult<ClienteListResponse>> ExecuteAllAtivosAsync(
        CancellationToken cancellationToken = default)
    {
        var clientes = await _clienteRepository.GetAllAtivosAsync(cancellationToken);

        var items = clientes.Select(MapToResponse).ToList();
        var response = new ClienteListResponse(items, items.Count);

        return new BusinessResult<ClienteListResponse>(response);
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
