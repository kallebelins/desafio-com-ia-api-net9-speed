using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Lab10.Application.DTOs;
using Lab10.Domain.Enums;

namespace Lab10.Application.Commands.Vendas;

/// <summary>
/// Command para finalizar uma venda (com pagamento)
/// </summary>
public record FinalizarVendaCommand(
    int VendaId,
    MetodoPagamento MetodoPagamento) : Mvp24Hours.Infrastructure.Cqrs.Abstractions.IMediatorCommand<IBusinessResult<VendaDto>>;
