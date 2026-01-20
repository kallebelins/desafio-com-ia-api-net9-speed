using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab03.Application.Commands;

/// <summary>
/// Command para excluir um produto
/// </summary>
public record DeleteProdutoCommand(int Id) : IMediatorCommand<bool>;
