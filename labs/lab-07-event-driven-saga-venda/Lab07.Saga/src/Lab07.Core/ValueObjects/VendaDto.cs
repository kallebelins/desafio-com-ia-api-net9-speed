using Lab07.Core.Enums;

namespace Lab07.Core.ValueObjects;

/// <summary>
/// DTO para transferência de dados de Venda
/// </summary>
public record VendaDto
{
    public Guid Id { get; init; }
    public Guid ClienteId { get; init; }
    public string? ClienteNome { get; init; }
    public decimal ValorTotal { get; init; }
    public VendaStatus Status { get; init; }
    public string? MotivoFalha { get; init; }
    public DateTime? DataConfirmacao { get; init; }
    public DateTime? DataCancelamento { get; init; }
    public DateTime Created { get; init; }
    public List<ItemVendaDto> Itens { get; init; } = new();
}
