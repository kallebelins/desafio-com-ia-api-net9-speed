using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Lab10.Application.DTOs;

namespace Lab10.Application.Commands.Vendas;

/// <summary>
/// Command para iniciar uma nova venda (Saga)
/// </summary>
public record IniciarVendaCommand(
    int ClienteId,
    IEnumerable<ItemVendaCreateDto> Itens) : Mvp24Hours.Infrastructure.Cqrs.Abstractions.IMediatorCommand<IBusinessResult<VendaDto>>;
