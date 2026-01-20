using Lab08.Application.DTOs;
using Lab08.Application.Interfaces;
using Lab08.Domain.Entities;
using Lab08.Domain.Interfaces;

namespace Lab08.Application.UseCases.Produtos.ListProdutos;

/// <summary>
/// Input para listar produtos
/// </summary>
public record ListProdutosInput
{
    public bool ApenasAtivos { get; init; } = false;
    public int? CategoriaId { get; init; }
}

/// <summary>
/// Output da listagem de produtos
/// </summary>
public record ListProdutosOutput
{
    public IReadOnlyList<ProdutoResumoDto> Produtos { get; init; } = [];
    public int Total { get; init; }
}

/// <summary>
/// Use Case para listar produtos
/// </summary>
public class ListProdutosUseCase : IUseCase<ListProdutosInput, ListProdutosOutput>
{
    private readonly IUnitOfWork _unitOfWork;

    public ListProdutosUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ListProdutosOutput> ExecuteAsync(ListProdutosInput input, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Produto> produtos;

        if (input.CategoriaId.HasValue)
        {
            produtos = await _unitOfWork.Produtos.GetByCategoriaAsync(input.CategoriaId.Value, cancellationToken);
            if (input.ApenasAtivos)
                produtos = produtos.Where(p => p.Ativo).ToList();
        }
        else
        {
            produtos = input.ApenasAtivos
                ? await _unitOfWork.Produtos.GetAtivosAsync(cancellationToken)
                : await _unitOfWork.Produtos.GetAllAsync(cancellationToken);
        }

        var dtos = produtos.Select(MapToResumoDto).ToList();

        return new ListProdutosOutput
        {
            Produtos = dtos,
            Total = dtos.Count
        };
    }

    private static ProdutoResumoDto MapToResumoDto(Produto produto)
    {
        return new ProdutoResumoDto
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Preco = produto.Preco.Valor,
            PrecoFormatado = produto.Preco.ToStringFormatado(),
            Estoque = produto.Estoque,
            Ativo = produto.Ativo
        };
    }
}
