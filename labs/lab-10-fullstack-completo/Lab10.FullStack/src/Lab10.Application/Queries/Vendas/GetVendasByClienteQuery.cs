using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Lab10.Application.DTOs;

namespace Lab10.Application.Queries.Vendas;

/// <summary>
/// Query para listar vendas de um cliente
/// </summary>
public record GetVendasByClienteQuery(int ClienteId) 
    : Mvp24Hours.Infrastructure.Cqrs.Abstractions.IMediatorQuery<IBusinessResult<IEnumerable<VendaResumoDto>>>;
