using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Lab10.Application.DTOs;

namespace Lab10.Application.Queries.Produtos;

/// <summary>
/// Query para buscar produto por ID
/// </summary>
public record GetProdutoByIdQuery(int Id) 
    : Mvp24Hours.Infrastructure.Cqrs.Abstractions.IMediatorQuery<IBusinessResult<ProdutoDto>>;
