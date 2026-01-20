using Lab09.Application.DTOs;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab09.Application.Commands;

/// <summary>
/// Command para cancelar uma venda
/// </summary>
public record CancelarVendaCommand : IMediatorCommand<VendaDto>
{
    public Guid VendaId { get; init; }
    public string Motivo { get; init; } = string.Empty;
}
