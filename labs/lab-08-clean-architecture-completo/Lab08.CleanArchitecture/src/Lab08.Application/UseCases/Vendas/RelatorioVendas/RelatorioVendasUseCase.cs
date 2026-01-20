using Lab08.Application.DTOs;
using Lab08.Application.Interfaces;
using Lab08.Domain.Enums;
using Lab08.Domain.Interfaces;

namespace Lab08.Application.UseCases.Vendas.RelatorioVendas;

/// <summary>
/// Input para relatório de vendas
/// </summary>
public record RelatorioVendasInput
{
    public DateTime DataInicio { get; init; }
    public DateTime DataFim { get; init; }
    public int TopProdutos { get; init; } = 10;
}

/// <summary>
/// Output do relatório de vendas
/// </summary>
public record RelatorioVendasOutput
{
    public bool Success { get; init; }
    public RelatorioVendasDto? Relatorio { get; init; }
    public string? ErrorMessage { get; init; }

    public static RelatorioVendasOutput Ok(RelatorioVendasDto relatorio)
        => new() { Success = true, Relatorio = relatorio };

    public static RelatorioVendasOutput Error(string message)
        => new() { Success = false, ErrorMessage = message };
}

/// <summary>
/// Use Case para gerar relatório de vendas por período
/// </summary>
public class RelatorioVendasUseCase : IUseCase<RelatorioVendasInput, RelatorioVendasOutput>
{
    private readonly IUnitOfWork _unitOfWork;

    public RelatorioVendasUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RelatorioVendasOutput> ExecuteAsync(RelatorioVendasInput input, CancellationToken cancellationToken = default)
    {
        try
        {
            if (input.DataInicio > input.DataFim)
                return RelatorioVendasOutput.Error("Data início não pode ser maior que data fim");

            // Buscar vendas do período com itens
            var vendas = await _unitOfWork.Vendas.GetByPeriodoComItensAsync(
                input.DataInicio, 
                input.DataFim.AddDays(1).AddTicks(-1), // Incluir todo o último dia
                cancellationToken);

            // Calcular totais
            var vendasConfirmadas = vendas.Where(v => v.Status == StatusVenda.Confirmada || v.Status == StatusVenda.Entregue).ToList();
            var vendasCanceladas = vendas.Where(v => v.Status == StatusVenda.Cancelada).ToList();

            var valorTotal = vendasConfirmadas.Sum(v => v.Total.Valor);
            var mediaVenda = vendasConfirmadas.Count > 0 ? valorTotal / vendasConfirmadas.Count : 0;

            // Calcular produtos mais vendidos (apenas de vendas confirmadas/entregues)
            var produtosMaisVendidos = vendasConfirmadas
                .SelectMany(v => v.Itens)
                .GroupBy(i => new { i.ProdutoId, i.Produto?.Nome })
                .Select(g => new ProdutoMaisVendidoDto
                {
                    ProdutoId = g.Key.ProdutoId,
                    ProdutoNome = g.Key.Nome ?? "Produto não encontrado",
                    QuantidadeVendida = g.Sum(i => i.Quantidade),
                    ValorTotalVendido = g.Sum(i => i.Subtotal.Valor),
                    ValorTotalFormatado = $"R$ {g.Sum(i => i.Subtotal.Valor):N2}"
                })
                .OrderByDescending(p => p.QuantidadeVendida)
                .Take(input.TopProdutos)
                .ToList();

            // Mapear vendas resumidas
            var vendasResumo = vendas.Select(v => new VendaResumoDto
            {
                Id = v.Id,
                ClienteNome = v.Cliente?.Nome ?? "Cliente não encontrado",
                DataVenda = v.DataVenda,
                Status = v.Status.ToString(),
                Total = v.Total.Valor,
                TotalFormatado = v.Total.ToStringFormatado(),
                QuantidadeItens = v.Itens.Count
            }).ToList();

            var relatorio = new RelatorioVendasDto
            {
                DataInicio = input.DataInicio,
                DataFim = input.DataFim,
                TotalVendas = vendas.Count,
                VendasConfirmadas = vendasConfirmadas.Count,
                VendasCanceladas = vendasCanceladas.Count,
                ValorTotal = valorTotal,
                ValorTotalFormatado = $"R$ {valorTotal:N2}",
                MediaPorVenda = mediaVenda,
                MediaPorVendaFormatado = $"R$ {mediaVenda:N2}",
                ProdutosMaisVendidos = produtosMaisVendidos,
                Vendas = vendasResumo
            };

            return RelatorioVendasOutput.Ok(relatorio);
        }
        catch (Exception ex)
        {
            return RelatorioVendasOutput.Error($"Erro ao gerar relatório: {ex.Message}");
        }
    }
}
