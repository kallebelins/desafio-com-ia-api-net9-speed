using Lab03.Core.ValueObjects;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab03.Application.Commands;

/// <summary>
/// Command para atualizar um produto existente
/// </summary>
public record UpdateProdutoCommand(
    int Id,
    string Nome,
    string? Descricao,
    decimal Preco,
    string Categoria,
    int Estoque,
    bool Ativo
) : IMediatorCommand<ProdutoDto?>;
