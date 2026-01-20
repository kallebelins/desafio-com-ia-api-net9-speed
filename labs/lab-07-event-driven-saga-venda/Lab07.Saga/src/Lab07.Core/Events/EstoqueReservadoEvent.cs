namespace Lab07.Core.Events;

/// <summary>
/// Evento disparado quando o estoque é reservado com sucesso
/// </summary>
public record EstoqueReservadoEvent
{
    public Guid VendaId { get; init; }
    public List<ReservaEstoqueData> Reservas { get; init; } = new();
    public DateTime OcorridoEm { get; init; } = DateTime.UtcNow;
}

public record ReservaEstoqueData
{
    public Guid ProdutoId { get; init; }
    public int Quantidade { get; init; }
}
