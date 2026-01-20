using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Lab10.Application.DTOs;

namespace Lab10.Application.Queries.Produtos;

/// <summary>
/// Query para listar produtos por categoria
/// </summary>
public record GetProdutosByCategoriaQuery(int CategoriaId) 
    : Mvp24Hours.Infrastructure.Cqrs.Abstractions.IMediatorQuery<IBusinessResult<IEnumerable<ProdutoDto>>>;
