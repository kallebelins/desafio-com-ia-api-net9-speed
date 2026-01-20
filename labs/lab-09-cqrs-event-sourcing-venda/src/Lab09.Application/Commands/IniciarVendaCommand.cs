using Lab09.Application.DTOs;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab09.Application.Commands;

/// <summary>
/// Command para iniciar uma nova venda
/// </summary>
public record IniciarVendaCommand : IMediatorCommand<VendaDto>
{
    public Guid ClienteId { get; init; }
}
