using Lab08.Application.DTOs;
using Lab08.Application.Interfaces;
using Lab08.Domain.Entities;
using Lab08.Domain.Exceptions;
using Lab08.Domain.Interfaces;
using Lab08.Domain.ValueObjects;

namespace Lab08.Application.UseCases.Clientes.CreateCliente;

/// <summary>
/// Use Case para criar um novo cliente
/// </summary>
public class CreateClienteUseCase : IUseCase<CreateClienteInput, CreateClienteOutput>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateClienteUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateClienteOutput> ExecuteAsync(CreateClienteInput input, CancellationToken cancellationToken = default)
    {
        try
        {
            // Criar Value Objects
            var email = Email.Create(input.Email);
            var cpf = Cpf.Create(input.Cpf);

            // Validar unicidade
            if (await _unitOfWork.Clientes.ExisteCpfAsync(cpf, cancellationToken: cancellationToken))
                return CreateClienteOutput.Error("CPF já cadastrado");

            if (await _unitOfWork.Clientes.ExisteEmailAsync(email, cancellationToken: cancellationToken))
                return CreateClienteOutput.Error("Email já cadastrado");

            // Criar endereço se fornecido
            Endereco? endereco = null;
            if (input.TemEndereco)
            {
                endereco = Endereco.Create(
                    input.Logradouro!,
                    input.Numero!,
                    input.Complemento,
                    input.Bairro!,
                    input.Cidade!,
                    input.Estado!,
                    input.Cep!
                );
            }

            // Criar cliente
            var cliente = new Cliente(input.Nome, email, cpf, endereco);

            // Persistir
            await _unitOfWork.Clientes.AddAsync(cliente, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Mapear para DTO
            var dto = MapToDto(cliente);

            return CreateClienteOutput.Ok(dto);
        }
        catch (DomainException ex)
        {
            return CreateClienteOutput.Error(ex.Message);
        }
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
