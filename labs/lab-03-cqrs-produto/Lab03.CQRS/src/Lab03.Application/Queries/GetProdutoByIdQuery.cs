using Lab03.Core.ValueObjects;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab03.Application.Queries;

/// <summary>
/// Query para buscar um produto por ID
/// </summary>
public record GetProdutoByIdQuery(int Id) : IMediatorQuery<ProdutoDto?>;
