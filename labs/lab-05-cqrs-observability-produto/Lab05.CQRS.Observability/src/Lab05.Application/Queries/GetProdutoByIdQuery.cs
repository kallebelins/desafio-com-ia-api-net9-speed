using Lab05.Core.ValueObjects;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab05.Application.Queries;

/// <summary>
/// Query para buscar um produto por ID
/// </summary>
public record GetProdutoByIdQuery : IMediatorQuery<IBusinessResult<ProdutoDto>>
{
    public Guid Id { get; init; }

    public GetProdutoByIdQuery() { }

    public GetProdutoByIdQuery(Guid id)
    {
        Id = id;
    }
}
