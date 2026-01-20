using Lab08.Application.DTOs;
using Lab08.Application.Interfaces;
using Lab08.Domain.Entities;
using Lab08.Domain.Interfaces;

namespace Lab08.Application.UseCases.Clientes.GetCliente;

/// <summary>
/// Input para buscar cliente
/// </summary>
public record GetClienteInput(int Id);

/// <summary>
/// Output da busca de cliente
/// </summary>
public record GetClienteOutput
{
    public bool Success { get; init; }
    public ClienteDto? Cliente { get; init; }
    public string? ErrorMessage { get; init; }

    public static GetClienteOutput Ok(ClienteDto cliente)
        => new() { Success = true, Cliente = cliente };

    public static GetClienteOutput NotFound()
        => new() { Success = false, ErrorMessage = "Cliente não encontrado" };
}

/// <summary>
/// Use Case para buscar um cliente por ID
/// </summary>
public class GetClienteUseCase : IUseCase<GetClienteInput, GetClienteOutput>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetClienteUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetClienteOutput> ExecuteAsync(GetClienteInput input, CancellationToken cancellationToken = default)
    {
        var cliente = await _unitOfWork.Clientes.GetByIdAsync(input.Id, cancellationToken);

        if (cliente == null)
            return GetClienteOutput.NotFound();

        var dto = MapToDto(cliente);
        return GetClienteOutput.Ok(dto);
    }

    private static ClienteDto MapToDto(Cliente cliente)
    {
        return new ClienteDto
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            Email = cliente.Email.Valor,
            Cpf = cliente.Cpf.Valor,
            CpfFormatado = cliente.Cpf.Formatado,
            Endereco = cliente.Endereco != null ? new EnderecoDto
            {
                Logradouro = cliente.Endereco.Logradouro,
                Numero = cliente.Endereco.Numero,
                Complemento = cliente.Endereco.Complemento,
                Bairro = cliente.Endereco.Bairro,
                Cidade = cliente.Endereco.Cidade,
                Estado = cliente.Endereco.Estado,
                Cep = cliente.Endereco.CepFormatado,
                EnderecoCompleto = cliente.Endereco.EnderecoCompleto
            } : null,
            Ativo = cliente.Ativo,
            DataCadastro = cliente.DataCadastro,
            DataAtualizacao = cliente.DataAtualizacao
        };
    }
}
