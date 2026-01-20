namespace Lab10.Application.DTOs;

public record RelatorioVendasDto(
    DateTime Inicio,
    DateTime Fim,
    int TotalVendas,
    decimal ValorTotal,
    decimal TicketMedio,
    int VendasFinalizadas,
    int VendasCanceladas,
    int VendasPendentes);

public record RelatorioVendasPorPeriodoDto(
    DateTime Data,
    int TotalVendas,
    decimal ValorTotal);

public record RelatorioProdutosMaisVendidosDto(
    int ProdutoId,
    string ProdutoNome,
    int QuantidadeVendida,
    decimal ValorTotal);
