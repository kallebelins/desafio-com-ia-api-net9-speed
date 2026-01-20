using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Lab10.Application.DTOs;

namespace Lab10.Application.Commands.Produtos;

/// <summary>
/// Command para adicionar estoque a um produto
/// </summary>
public record AtualizarEstoqueCommand(
    int ProdutoId,
    int Quantidade) : Mvp24Hours.Infrastructure.Cqrs.Abstractions.IMediatorCommand<IBusinessResult<ProdutoDto>>;
