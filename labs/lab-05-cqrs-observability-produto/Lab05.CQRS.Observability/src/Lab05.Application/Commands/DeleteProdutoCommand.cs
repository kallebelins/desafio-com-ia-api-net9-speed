using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab05.Application.Commands;

/// <summary>
/// Command para deletar um produto
/// </summary>
public record DeleteProdutoCommand : IMediatorCommand<IBusinessResult<bool>>
{
    public Guid Id { get; init; }

    public DeleteProdutoCommand() { }

    public DeleteProdutoCommand(Guid id)
    {
        Id = id;
    }
}
