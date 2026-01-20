namespace Lab07.Core.ValueObjects;

/// <summary>
/// DTO para transferência de dados de Item de Venda
/// </summary>
public record ItemVendaDto
{
    public Guid Id { get; init; }
    public Guid ProdutoId { get; init; }
    public string? ProdutoNome { get; init; }
    public int Quantidade { get; init; }
    public decimal PrecoUnitario { get; init; }
    public decimal ValorTotal { get; init; }
}
