using Lab03.Application.Commands;
using Lab03.Core.Entities;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab03.Application.Handlers.Commands;

/// <summary>
/// Handler para excluir um produto
/// </summary>
public class DeleteProdutoCommandHandler : IMediatorCommandHandler<DeleteProdutoCommand, bool>
{
    private readonly IRepositoryAsync<Produto> _repository;
    private readonly IUnitOfWorkAsync _unitOfWork;

    public DeleteProdutoCommandHandler(
        IRepositoryAsync<Produto> repository,
        IUnitOfWorkAsync unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteProdutoCommand request, CancellationToken cancellationToken)
    {
        var produto = await _repository.GetByIdAsync(request.Id);

        if (produto is null)
            return false;

        await _repository.RemoveByIdAsync(request.Id);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
