using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Lab10.Application.DTOs;

namespace Lab10.Application.Queries.Clientes;

/// <summary>
/// Query para listar todos os clientes
/// </summary>
public record GetAllClientesQuery(bool ApenasAtivos = false) 
    : Mvp24Hours.Infrastructure.Cqrs.Abstractions.IMediatorQuery<IBusinessResult<IEnumerable<ClienteDto>>>;
