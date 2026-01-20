using Lab09.Application.DTOs;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab09.Application.Queries;

/// <summary>
/// Query para obter uma venda por ID
/// </summary>
public record GetVendaByIdQuery : IMediatorQuery<VendaDto?>
{
    public Guid VendaId { get; init; }
}
