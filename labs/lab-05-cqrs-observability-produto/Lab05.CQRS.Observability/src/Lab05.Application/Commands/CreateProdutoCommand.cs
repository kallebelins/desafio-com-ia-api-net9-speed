using Lab05.Core.ValueObjects;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab05.Application.Commands;

/// <summary>
/// Command para criar um novo produto
/// </summary>
public record CreateProdutoCommand : IMediatorCommand<IBusinessResult<ProdutoDto>>
{
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public decimal Preco { get; init; }
    public string SKU { get; init; } = string.Empty;
    public string Categoria { get; init; } = string.Empty;

    public CreateProdutoCommand() { }

    public CreateProdutoCommand(CreateProdutoDto dto)
    {
        Nome = dto.Nome;
        Descricao = dto.Descricao;
        Preco = dto.Preco;
        SKU = dto.SKU;
        Categoria = dto.Categoria;
    }
}
