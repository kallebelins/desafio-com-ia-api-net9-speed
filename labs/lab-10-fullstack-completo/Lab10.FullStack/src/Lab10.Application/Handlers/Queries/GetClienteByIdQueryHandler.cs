using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Core.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;
using Lab10.Application.DTOs;
using Lab10.Application.Queries.Clientes;
using Lab10.Domain.Interfaces;

namespace Lab10.Application.Handlers.Queries;

public class GetClienteByIdQueryHandler : IMediatorQueryHandler<GetClienteByIdQuery, IBusinessResult<ClienteDto>>
{
    private readonly IClienteRepository _clienteRepository;

    public GetClienteByIdQueryHandler(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<IBusinessResult<ClienteDto>> Handle(GetClienteByIdQuery request, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (cliente == null)
            return CreateErrorResult<ClienteDto>("Cliente não encontrado");

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

        return new BusinessResult<ClienteDto>(dto);
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
