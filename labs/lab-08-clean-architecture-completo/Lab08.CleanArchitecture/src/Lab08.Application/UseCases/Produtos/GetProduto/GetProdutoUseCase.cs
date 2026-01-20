using Lab08.Application.DTOs;
using Lab08.Application.Interfaces;
using Lab08.Domain.Entities;
using Lab08.Domain.Interfaces;

namespace Lab08.Application.UseCases.Produtos.GetProduto;

/// <summary>
/// Input para buscar produto
/// </summary>
public record GetProdutoInput(int Id);

/// <summary>
/// Output da busca de produto
/// </summary>
public record GetProdutoOutput
{
    public bool Success { get; init; }
    public ProdutoDto? Produto { get; init; }
    public string? ErrorMessage { get; init; }

    public static GetProdutoOutput Ok(ProdutoDto produto)
        => new() { Success = true, Produto = produto };

    public static GetProdutoOutput NotFound()
        => new() { Success = false, ErrorMessage = "Produto não encontrado" };
}

/// <summary>
/// Use Case para buscar um produto por ID
/// </summary>
public class GetProdutoUseCase : IUseCase<GetProdutoInput, GetProdutoOutput>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProdutoUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetProdutoOutput> ExecuteAsync(GetProdutoInput input, CancellationToken cancellationToken = default)
    {
        var produto = await _unitOfWork.Produtos.GetByIdWithCategoriaAsync(input.Id, cancellationToken);

        if (produto == null)
            return GetProdutoOutput.NotFound();

        var dto = MapToDto(produto);
        return GetProdutoOutput.Ok(dto);
    }

    private static ProdutoDto MapToDto(Produto produto)
    {
        return new ProdutoDto
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Preco = produto.Preco.Valor,
            PrecoFormatado = produto.Preco.ToStringFormatado(),
            Estoque = produto.Estoque,
            Ativo = produto.Ativo,
            CategoriaId = produto.CategoriaId,
            CategoriaNome = produto.Categoria?.Nome ?? string.Empty,
            DataCadastro = produto.DataCadastro,
            DataAtualizacao = produto.DataAtualizacao
        };
    }
}
