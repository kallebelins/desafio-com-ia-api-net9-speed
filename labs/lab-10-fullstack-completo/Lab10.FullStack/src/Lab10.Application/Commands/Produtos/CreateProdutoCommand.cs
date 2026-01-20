using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Lab10.Application.DTOs;

namespace Lab10.Application.Commands.Produtos;

/// <summary>
/// Command para criar um novo produto
/// </summary>
public record CreateProdutoCommand(
    string Nome,
    string? Descricao,
    decimal PrecoUnitario,
    int EstoqueInicial,
    int CategoriaId) : Mvp24Hours.Infrastructure.Cqrs.Abstractions.IMediatorCommand<IBusinessResult<ProdutoDto>>;
