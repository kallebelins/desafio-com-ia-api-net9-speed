using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Lab10.Application.DTOs;

namespace Lab10.Application.Queries.Clientes;

/// <summary>
/// Query para buscar cliente por ID
/// </summary>
public record GetClienteByIdQuery(int Id) 
    : Mvp24Hours.Infrastructure.Cqrs.Abstractions.IMediatorQuery<IBusinessResult<ClienteDto>>;
