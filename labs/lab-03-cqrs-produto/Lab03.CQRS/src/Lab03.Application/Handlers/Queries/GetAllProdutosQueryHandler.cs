using Lab03.Application.Queries;
using Lab03.Core.Entities;
using Lab03.Core.ValueObjects;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab03.Application.Handlers.Queries;

/// <summary>
/// Handler para buscar todos os produtos
/// </summary>
public class GetAllProdutosQueryHandler : IMediatorQueryHandler<GetAllProdutosQuery, IEnumerable<ProdutoDto>>
{
    private readonly IRepositoryAsync<Produto> _repository;

    public GetAllProdutosQueryHandler(IRepositoryAsync<Produto> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ProdutoDto>> Handle(GetAllProdutosQuery request, CancellationToken cancellationToken)
    {
        IList<Produto> produtos;

        if (request.ApenasAtivos == true && !string.IsNullOrEmpty(request.Categoria))
        {
            produtos = await _repository.GetByAsync(p => p.Ativo && p.Categoria == request.Categoria);
        }
        else if (request.ApenasAtivos == true)
        {
            produtos = await _repository.GetByAsync(p => p.Ativo);
        }
        else if (!string.IsNullOrEmpty(request.Categoria))
        {
            produtos = await _repository.GetByAsync(p => p.Categoria == request.Categoria);
        }
        else
        {
            produtos = await _repository.ListAsync();
        }

        return ProdutoDto.FromEntities(produtos);
    }
}
