using Mvp24Hours.Core.Contract.ValueObjects.Logic;

namespace Lab10.Application.Commands.Vendas;

/// <summary>
/// Command para cancelar uma venda
/// </summary>
public record CancelarVendaCommand(
    int VendaId,
    string? Motivo) : Mvp24Hours.Infrastructure.Cqrs.Abstractions.IMediatorCommand<IBusinessResult<bool>>;
