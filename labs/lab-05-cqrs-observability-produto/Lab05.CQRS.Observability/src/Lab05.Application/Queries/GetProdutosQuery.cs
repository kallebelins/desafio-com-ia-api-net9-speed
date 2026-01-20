using Lab05.Core.ValueObjects;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab05.Application.Queries;

/// <summary>
/// Query para listar todos os produtos
/// </summary>
public record GetProdutosQuery : IMediatorQuery<IBusinessResult<IList<ProdutoDto>>>
{
    public bool? ApenasAtivos { get; init; }
    public string? Categoria { get; init; }

    public GetProdutosQuery() { }

    public GetProdutosQuery(bool? apenasAtivos = null, string? categoria = null)
    {
        ApenasAtivos = apenasAtivos;
        Categoria = categoria;
    }
}
