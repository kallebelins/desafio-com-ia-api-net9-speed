namespace Lab07.Core.Events;

/// <summary>
/// Evento disparado quando uma venda é iniciada
/// </summary>
public record VendaIniciadaEvent
{
    public Guid VendaId { get; init; }
    public Guid ClienteId { get; init; }
    public List<ItemVendaEventData> Itens { get; init; } = new();
    public DateTime OcorridoEm { get; init; } = DateTime.UtcNow;
}

public record ItemVendaEventData
{
    public Guid ProdutoId { get; init; }
    public int Quantidade { get; init; }
}
