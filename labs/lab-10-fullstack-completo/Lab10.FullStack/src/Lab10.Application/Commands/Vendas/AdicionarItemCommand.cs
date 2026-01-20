using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Lab10.Application.DTOs;

namespace Lab10.Application.Commands.Vendas;

/// <summary>
/// Command para adicionar item a uma venda
/// </summary>
public record AdicionarItemCommand(
    int VendaId,
    int ProdutoId,
    int Quantidade) : Mvp24Hours.Infrastructure.Cqrs.Abstractions.IMediatorCommand<IBusinessResult<VendaDto>>;
