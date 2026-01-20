using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Lab10.Application.DTOs;

namespace Lab10.Application.Commands.Clientes;

/// <summary>
/// Command para atualizar um cliente existente
/// </summary>
public record UpdateClienteCommand(
    int Id,
    string Nome,
    string Email,
    EnderecoDto? Endereco) : Mvp24Hours.Infrastructure.Cqrs.Abstractions.IMediatorCommand<IBusinessResult<ClienteDto>>;
