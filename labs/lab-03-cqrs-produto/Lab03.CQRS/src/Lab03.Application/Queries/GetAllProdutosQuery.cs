using Lab03.Core.ValueObjects;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab03.Application.Queries;

/// <summary>
/// Query para buscar todos os produtos
/// </summary>
public record GetAllProdutosQuery(
    bool? ApenasAtivos = true,
    string? Categoria = null
) : IMediatorQuery<IEnumerable<ProdutoDto>>;
