using Lab09.Application.DTOs;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab09.Application.Commands;

/// <summary>
/// Command para finalizar uma venda
/// </summary>
public record FinalizarVendaCommand : IMediatorCommand<VendaDto>
{
    public Guid VendaId { get; init; }
}
