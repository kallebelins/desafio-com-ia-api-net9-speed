using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Lab10.Application.DTOs;

namespace Lab10.Application.Queries.Vendas;

/// <summary>
/// Query para obter relatório de vendas por período
/// </summary>
public record GetRelatorioVendasQuery(DateTime Inicio, DateTime Fim) 
    : Mvp24Hours.Infrastructure.Cqrs.Abstractions.IMediatorQuery<IBusinessResult<RelatorioVendasDto>>;
