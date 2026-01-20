namespace Lab09.Application.Projections;

/// <summary>
/// Read Model para relatório de vendas (projeção agregada)
/// </summary>
public class RelatorioVendasReadModel
{
    public DateTime Data { get; set; }
    public int TotalVendas { get; set; }
    public int VendasFinalizadas { get; set; }
    public int VendasCanceladas { get; set; }
    public int VendasEmAndamento { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal TotalDescontos { get; set; }
    public decimal TicketMedio { get; set; }
    public DateTime LastUpdated { get; set; }
}
