namespace Lab09.Core.Events;

/// <summary>
/// Evento: Item foi removido da venda
/// </summary>
public sealed record ItemRemovidoEvent : DomainEventBase
{
    public Guid VendaId { get; init; }
    public Guid ProdutoId { get; init; }
}
