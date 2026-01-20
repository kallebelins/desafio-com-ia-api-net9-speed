namespace Lab08.Application.DTOs;

/// <summary>
/// DTO para Venda
/// </summary>
public record VendaDto
{
    public int Id { get; init; }
    public int ClienteId { get; init; }
    public string ClienteNome { get; init; } = string.Empty;
    public DateTime DataVenda { get; init; }
    public string Status { get; init; } = string.Empty;
    public decimal Total { get; init; }
    public string TotalFormatado { get; init; } = string.Empty;
    public string? Observacao { get; init; }
    public IReadOnlyList<ItemVendaDto> Itens { get; init; } = [];
}

/// <summary>
/// DTO resumido para Venda (listagens)
/// </summary>
public record VendaResumoDto
{
    public int Id { get; init; }
    public string ClienteNome { get; init; } = string.Empty;
    public DateTime DataVenda { get; init; }
    public string Status { get; init; } = string.Empty;
    public decimal Total { get; init; }
    public string TotalFormatado { get; init; } = string.Empty;
    public int QuantidadeItens { get; init; }
}

/// <summary>
/// DTO para Item de Venda
/// </summary>
public record ItemVendaDto
{
    public int Id { get; init; }
    public int ProdutoId { get; init; }
    public string ProdutoNome { get; init; } = string.Empty;
    public int Quantidade { get; init; }
    public decimal PrecoUnitario { get; init; }
    public string PrecoUnitarioFormatado { get; init; } = string.Empty;
    public decimal Subtotal { get; init; }
    public string SubtotalFormatado { get; init; } = string.Empty;
}

/// <summary>
/// DTO para Relatório de Vendas
/// </summary>
public record RelatorioVendasDto
{
    public DateTime DataInicio { get; init; }
    public DateTime DataFim { get; init; }
    public int TotalVendas { get; init; }
    public int VendasConfirmadas { get; init; }
    public int VendasCanceladas { get; init; }
    public decimal ValorTotal { get; init; }
    public string ValorTotalFormatado { get; init; } = string.Empty;
    public decimal MediaPorVenda { get; init; }
    public string MediaPorVendaFormatado { get; init; } = string.Empty;
    public IReadOnlyList<ProdutoMaisVendidoDto> ProdutosMaisVendidos { get; init; } = [];
    public IReadOnlyList<VendaResumoDto> Vendas { get; init; } = [];
}
