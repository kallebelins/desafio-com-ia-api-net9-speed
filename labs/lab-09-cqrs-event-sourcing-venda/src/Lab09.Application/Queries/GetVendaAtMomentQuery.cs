using Lab09.Application.DTOs;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab09.Application.Queries;

/// <summary>
/// Query para reconstruir o estado de uma venda em um momento específico no tempo (Time Travel)
/// </summary>
public record GetVendaAtMomentQuery : IMediatorQuery<VendaDto?>
{
    public Guid VendaId { get; init; }
    
    /// <summary>
    /// Data/hora para reconstruir o estado
    /// </summary>
    public DateTime Momento { get; init; }
}
