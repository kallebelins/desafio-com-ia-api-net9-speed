using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Core.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;
using Lab10.Application.DTOs;
using Lab10.Application.Queries.Clientes;
using Lab10.Domain.Interfaces;

namespace Lab10.Application.Handlers.Queries;

public class GetAllClientesQueryHandler : IMediatorQueryHandler<GetAllClientesQuery, IBusinessResult<IEnumerable<ClienteDto>>>
{
    private readonly IClienteRepository _clienteRepository;

    public GetAllClientesQueryHandler(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<IBusinessResult<IEnumerable<ClienteDto>>> Handle(GetAllClientesQuery request, CancellationToken cancellationToken)
    {
        var clientes = request.ApenasAtivos 
            ? await _clienteRepository.GetAtivosAsync(cancellationToken)
            : await _clienteRepository.GetAllAsync(cancellationToken);

        var dtos = clientes.Select(c =>
        {
            EnderecoDto? enderecoDto = null;
            if (c.Endereco != null)
            {
                enderecoDto = new EnderecoDto(
                    c.Endereco.Logradouro,
                    c.Endereco.Numero,
                    c.Endereco.Complemento,
                    c.Endereco.Bairro,
                    c.Endereco.Cidade,
                    c.Endereco.Estado,
                    c.Endereco.CepFormatado);
            }

            return new ClienteDto(
                c.Id,
                c.Nome,
                c.Email.Valor,
                c.Cpf.Formatado,
                enderecoDto,
                c.Ativo,
                c.DataCadastro);
        });

        return new BusinessResult<IEnumerable<ClienteDto>>(dtos);
    }
}
