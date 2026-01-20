using Lab03.Application.Commands;
using Lab03.Core.Entities;
using Lab03.Core.ValueObjects;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab03.Application.Handlers.Commands;

/// <summary>
/// Handler para criar um novo produto
/// </summary>
public class CreateProdutoCommandHandler : IMediatorCommandHandler<CreateProdutoCommand, ProdutoDto>
{
    private readonly IRepositoryAsync<Produto> _repository;
    private readonly IUnitOfWorkAsync _unitOfWork;

    public CreateProdutoCommandHandler(
        IRepositoryAsync<Produto> repository,
        IUnitOfWorkAsync unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProdutoDto> Handle(CreateProdutoCommand request, CancellationToken cancellationToken)
    {
        var produto = new Produto
        {
            Nome = request.Nome,
            Descricao = request.Descricao,
            Preco = request.Preco,
            Categoria = request.Categoria,
            Estoque = request.Estoque,
            Ativo = true,
            Created = DateTime.UtcNow
        };

        await _repository.AddAsync(produto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ProdutoDto.FromEntity(produto);
    }
}
