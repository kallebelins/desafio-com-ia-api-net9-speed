using Lab03.Application.Queries;
using Lab03.Core.Entities;
using Lab03.Core.ValueObjects;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab03.Application.Handlers.Queries;

/// <summary>
/// Handler para buscar um produto por ID
/// </summary>
public class GetProdutoByIdQueryHandler : IMediatorQueryHandler<GetProdutoByIdQuery, ProdutoDto?>
{
    private readonly IRepositoryAsync<Produto> _repository;

    public GetProdutoByIdQueryHandler(IRepositoryAsync<Produto> repository)
    {
        _repository = repository;
    }

    public async Task<ProdutoDto?> Handle(GetProdutoByIdQuery request, CancellationToken cancellationToken)
    {
        var produto = await _repository.GetByIdAsync(request.Id);

        if (produto is null)
            return null;

        return ProdutoDto.FromEntity(produto);
    }
}
