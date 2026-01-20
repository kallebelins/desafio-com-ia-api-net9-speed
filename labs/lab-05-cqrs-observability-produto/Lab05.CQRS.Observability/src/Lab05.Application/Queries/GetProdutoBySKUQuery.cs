using Lab05.Core.ValueObjects;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab05.Application.Queries;

/// <summary>
/// Query para buscar um produto por SKU
/// </summary>
public record GetProdutoBySKUQuery : IMediatorQuery<IBusinessResult<ProdutoDto>>
{
    public string SKU { get; init; } = string.Empty;

    public GetProdutoBySKUQuery() { }

    public GetProdutoBySKUQuery(string sku)
    {
        SKU = sku;
    }
}
