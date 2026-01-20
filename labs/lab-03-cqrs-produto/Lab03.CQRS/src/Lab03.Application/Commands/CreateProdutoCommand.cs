using Lab03.Core.ValueObjects;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab03.Application.Commands;

/// <summary>
/// Command para criar um novo produto
/// </summary>
public record CreateProdutoCommand(
    string Nome,
    string? Descricao,
    decimal Preco,
    string Categoria,
    int Estoque
) : IMediatorCommand<ProdutoDto>;
