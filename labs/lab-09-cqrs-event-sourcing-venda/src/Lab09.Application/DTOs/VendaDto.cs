namespace Lab09.Application.DTOs;

/// <summary>
/// DTO para representar uma Venda
/// </summary>
public record VendaDto
{
    public Guid Id { get; init; }
    public Guid ClienteId { get; init; }
    public List<ItemVendaDto> Itens { get; init; } = new();
    public decimal Subtotal { get; init; }
    public decimal Desconto { get; init; }
    public decimal Total { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime? DataInicio { get; init; }
    public DateTime? DataFinalizacao { get; init; }
    public DateTime? DataCancelamento { get; init; }
    public int Version { get; init; }
}

/// <summary>
/// DTO para representar um Item de Venda
/// </summary>
public record ItemVendaDto
{
    public Guid ProdutoId { get; init; }
    public string ProdutoNome { get; init; } = string.Empty;
    public int Quantidade { get; init; }
    public decimal PrecoUnitario { get; init; }
    public decimal Subtotal { get; init; }
}
