using Lab03.Application.Commands;
using Lab03.Core.Entities;
using Lab03.Core.ValueObjects;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab03.Application.Handlers.Commands;

/// <summary>
/// Handler para atualizar um produto existente
/// </summary>
public class UpdateProdutoCommandHandler : IMediatorCommandHandler<UpdateProdutoCommand, ProdutoDto?>
{
    private readonly IRepositoryAsync<Produto> _repository;
    private readonly IUnitOfWorkAsync _unitOfWork;

    public UpdateProdutoCommandHandler(
        IRepositoryAsync<Produto> repository,
        IUnitOfWorkAsync unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProdutoDto?> Handle(UpdateProdutoCommand request, CancellationToken cancellationToken)
    {
        var produto = await _repository.GetByIdAsync(request.Id);

        if (produto is null)
            return null;

        produto.Nome = request.Nome;
        produto.Descricao = request.Descricao;
        produto.Preco = request.Preco;
        produto.Categoria = request.Categoria;
        produto.Estoque = request.Estoque;
        produto.Ativo = request.Ativo;
        produto.Modified = DateTime.UtcNow;

        await _repository.ModifyAsync(produto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ProdutoDto.FromEntity(produto);
    }
}
