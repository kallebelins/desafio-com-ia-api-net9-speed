using Lab08.Application.DTOs;
using Lab08.Application.Interfaces;
using Lab08.Domain.Entities;
using Lab08.Domain.Exceptions;
using Lab08.Domain.Interfaces;
using Lab08.Domain.ValueObjects;

namespace Lab08.Application.UseCases.Produtos.CreateProduto;

/// <summary>
/// Input para criação de produto
/// </summary>
public record CreateProdutoInput
{
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public decimal Preco { get; init; }
    public int Estoque { get; init; }
    public int CategoriaId { get; init; }
}

/// <summary>
/// Output da criação de produto
/// </summary>
public record CreateProdutoOutput
{
    public bool Success { get; init; }
    public ProdutoDto? Produto { get; init; }
    public string? ErrorMessage { get; init; }

    public static CreateProdutoOutput Ok(ProdutoDto produto)
        => new() { Success = true, Produto = produto };

    public static CreateProdutoOutput Error(string message)
        => new() { Success = false, ErrorMessage = message };
}

/// <summary>
/// Use Case para criar um novo produto
/// </summary>
public class CreateProdutoUseCase : IUseCase<CreateProdutoInput, CreateProdutoOutput>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProdutoUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateProdutoOutput> ExecuteAsync(CreateProdutoInput input, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validar categoria
            var categoria = await _unitOfWork.Categorias.GetByIdAsync(input.CategoriaId, cancellationToken);
            if (categoria == null)
                return CreateProdutoOutput.Error("Categoria não encontrada");

            if (!categoria.Ativo)
                return CreateProdutoOutput.Error("Categoria está inativa");

            // Validar unicidade do nome
            if (await _unitOfWork.Produtos.ExisteNomeAsync(input.Nome, cancellationToken: cancellationToken))
                return CreateProdutoOutput.Error("Já existe um produto com este nome");

            // Criar Value Object de preço
            var preco = Money.Create(input.Preco, "BRL");

            // Criar produto
            var produto = new Produto(input.Nome, input.Descricao, preco, input.Estoque, input.CategoriaId);

            // Persistir
            await _unitOfWork.Produtos.AddAsync(produto, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Mapear para DTO
            var dto = new ProdutoDto
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Preco = produto.Preco.Valor,
                PrecoFormatado = produto.Preco.ToStringFormatado(),
                Estoque = produto.Estoque,
                Ativo = produto.Ativo,
                CategoriaId = produto.CategoriaId,
                CategoriaNome = categoria.Nome,
                DataCadastro = produto.DataCadastro,
                DataAtualizacao = produto.DataAtualizacao
            };

            return CreateProdutoOutput.Ok(dto);
        }
        catch (DomainException ex)
        {
            return CreateProdutoOutput.Error(ex.Message);
        }
    }
}
