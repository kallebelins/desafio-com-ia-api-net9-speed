using Lab09.Application.DTOs;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab09.Application.Commands;

/// <summary>
/// Command para remover um item da venda
/// </summary>
public record RemoverItemCommand : IMediatorCommand<VendaDto>
{
    public Guid VendaId { get; init; }
    public Guid ProdutoId { get; init; }
}
