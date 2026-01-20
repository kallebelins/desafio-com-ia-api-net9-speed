using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Core.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;
using Lab10.Application.DTOs;
using Lab10.Application.Queries.Produtos;
using Lab10.Domain.Interfaces;

namespace Lab10.Application.Handlers.Queries;

public class GetProdutoByIdQueryHandler : IMediatorQueryHandler<GetProdutoByIdQuery, IBusinessResult<ProdutoDto>>
{
    private readonly IProdutoRepository _produtoRepository;

    public GetProdutoByIdQueryHandler(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<IBusinessResult<ProdutoDto>> Handle(GetProdutoByIdQuery request, CancellationToken cancellationToken)
    {
        var produto = await _produtoRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (produto == null)
            return CreateErrorResult<ProdutoDto>("Produto não encontrado");

        var dto = new ProdutoDto(
            produto.Id,
            produto.Nome,
            produto.Descricao,
            produto.PrecoUnitario.Valor,
            produto.EstoqueAtual,
            produto.EstoqueReservado,
            produto.EstoqueDisponivel,
            produto.CategoriaId,
            produto.Categoria?.Nome,
            produto.Ativo);

        return new BusinessResult<ProdutoDto>(dto);
    }

    private static IBusinessResult<T> CreateErrorResult<T>(string message)
    {
        IReadOnlyCollection<IMessageResult> messages = new List<IMessageResult>
        {
            new MessageResult(message, MessageType.Error)
        };
        return new BusinessResult<T>(default!, messages);
    }
}
