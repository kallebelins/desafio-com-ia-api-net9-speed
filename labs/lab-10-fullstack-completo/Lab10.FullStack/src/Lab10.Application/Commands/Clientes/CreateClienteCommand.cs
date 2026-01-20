using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Lab10.Application.DTOs;

namespace Lab10.Application.Commands.Clientes;

/// <summary>
/// Command para criar um novo cliente
/// </summary>
public record CreateClienteCommand(
    string Nome,
    string Email,
    string Cpf,
    EnderecoDto? Endereco) : Mvp24Hours.Infrastructure.Cqrs.Abstractions.IMediatorCommand<IBusinessResult<ClienteDto>>;
