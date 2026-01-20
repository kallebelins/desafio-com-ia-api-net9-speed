using Lab09.Application.DTOs;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab09.Application.Queries;

/// <summary>
/// Query para obter o histórico completo de eventos de uma venda
/// </summary>
public record GetVendaHistoryQuery : IMediatorQuery<VendaHistoryDto?>
{
    public Guid VendaId { get; init; }
}
