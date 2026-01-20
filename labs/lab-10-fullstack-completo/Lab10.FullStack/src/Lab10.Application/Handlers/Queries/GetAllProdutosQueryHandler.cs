using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Core.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;
using Lab10.Application.DTOs;
using Lab10.Application.Queries.Produtos;
using Lab10.Domain.Interfaces;

namespace Lab10.Application.Handlers.Queries;

public class GetAllProdutosQueryHandler : IMediatorQueryHandler<GetAllProdutosQuery, IBusinessResult<IEnumerable<ProdutoDto>>>
{
    private readonly IProdutoRepository _produtoRepository;

    public GetAllProdutosQueryHandler(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<IBusinessResult<IEnumerable<ProdutoDto>>> Handle(GetAllProdutosQuery request, CancellationToken cancellationToken)
    {
        var produtos = request.ApenasAtivos
            ? await _produtoRepository.GetAtivosAsync(cancellationToken)
            : await _produtoRepository.GetAllAsync(cancellationToken);

        if (request.ApenasComEstoque)
        {
            produtos = produtos.Where(p => p.EstoqueDisponivel > 0);
        }

        var dtos = produtos.Select(p => new ProdutoDto(
            p.Id,
            p.Nome,
            p.Descricao,
            p.PrecoUnitario.Valor,
            p.EstoqueAtual,
            p.EstoqueReservado,
            p.EstoqueDisponivel,
            p.CategoriaId,
            p.Categoria?.Nome,
            p.Ativo));

        return new BusinessResult<IEnumerable<ProdutoDto>>(dtos);
    }
}
