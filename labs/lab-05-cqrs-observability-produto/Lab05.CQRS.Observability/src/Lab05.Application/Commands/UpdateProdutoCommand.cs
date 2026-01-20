using Lab05.Core.ValueObjects;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab05.Application.Commands;

/// <summary>
/// Command para atualizar um produto existente
/// </summary>
public record UpdateProdutoCommand : IMediatorCommand<IBusinessResult<ProdutoDto>>
{
    public Guid Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public decimal Preco { get; init; }
    public string SKU { get; init; } = string.Empty;
    public string Categoria { get; init; } = string.Empty;
    public bool Ativo { get; init; }

    public UpdateProdutoCommand() { }

    public UpdateProdutoCommand(UpdateProdutoDto dto)
    {
        Id = dto.Id;
        Nome = dto.Nome;
        Descricao = dto.Descricao;
        Preco = dto.Preco;
        SKU = dto.SKU;
        Categoria = dto.Categoria;
        Ativo = dto.Ativo;
    }
}
