using Lab09.Application.DTOs;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab09.Application.Commands;

/// <summary>
/// Command para adicionar um item à venda
/// </summary>
public record AdicionarItemCommand : IMediatorCommand<VendaDto>
{
    public Guid VendaId { get; init; }
    public Guid ProdutoId { get; init; }
    public string ProdutoNome { get; init; } = string.Empty;
    public int Quantidade { get; init; }
    public decimal PrecoUnitario { get; init; }
}
