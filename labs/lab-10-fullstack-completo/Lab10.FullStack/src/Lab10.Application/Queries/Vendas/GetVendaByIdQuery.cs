using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Lab10.Application.DTOs;

namespace Lab10.Application.Queries.Vendas;

/// <summary>
/// Query para buscar venda por ID com todos os detalhes
/// </summary>
public record GetVendaByIdQuery(int Id) 
    : Mvp24Hours.Infrastructure.Cqrs.Abstractions.IMediatorQuery<IBusinessResult<VendaDto>>;
