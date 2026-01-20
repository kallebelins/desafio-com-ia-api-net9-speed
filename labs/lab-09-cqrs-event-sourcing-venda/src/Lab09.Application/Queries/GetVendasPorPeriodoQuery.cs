using Lab09.Application.DTOs;
using Lab09.Application.Projections;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab09.Application.Queries;

/// <summary>
/// Query para obter vendas por período (consulta no Read Model)
/// </summary>
public record GetVendasPorPeriodoQuery : IMediatorQuery<IEnumerable<VendaReadModel>>
{
    public DateTime? DataInicio { get; init; }
    public DateTime? DataFim { get; init; }
    public string? Status { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
