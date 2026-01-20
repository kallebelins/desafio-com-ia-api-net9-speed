using Lab04.Core.Entities;

namespace Lab04.Core.ValueObjects;

/// <summary>
/// DTO para retorno de dados do Cliente
/// </summary>
public record ClienteDto(
    int Id,
    string Nome,
    string Email,
    string CPF,
    string? Telefone,
    bool Ativo,
    DateTime DataCriacao)
{
    public static ClienteDto FromEntity(Cliente cliente)
    {
        return new ClienteDto(
            cliente.Id,
            cliente.Nome,
            cliente.Email,
            cliente.CPF,
            cliente.Telefone,
            cliente.Ativo,
            cliente.DataCriacao);
    }
}

/// <summary>
/// DTO para criação de Cliente
/// </summary>
public record ClienteCreateDto(
    string Nome,
    string Email,
    string CPF,
    string? Telefone);

/// <summary>
/// DTO para atualização de Cliente
/// </summary>
public record ClienteUpdateDto(
    string Nome,
    string Email,
    string? Telefone,
    bool Ativo);
