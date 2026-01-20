using Lab09.Core.Events;
using Lab09.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lab09.Application.Projections;

/// <summary>
/// Projeção que mantém relatórios agregados de vendas
/// </summary>
public class RelatorioVendasProjection : IProjection
{
    private readonly DbContext _context;

    public string Name => "RelatorioVendasProjection";

    public RelatorioVendasProjection(DbContext context)
    {
        _context = context;
    }

    public async Task HandleAsync(IDomainEvent @event, CancellationToken cancellationToken = default)
    {
        switch (@event)
        {
            case VendaIniciadaEvent e:
                await HandleVendaIniciada(e, cancellationToken);
                break;
            case VendaFinalizadaEvent e:
                await HandleVendaFinalizada(e, cancellationToken);
                break;
            case VendaCanceladaEvent e:
                await HandleVendaCancelada(e, cancellationToken);
                break;
            case DescontoAplicadoEvent e:
                await HandleDescontoAplicado(e, cancellationToken);
                break;
        }
    }

    private async Task HandleVendaIniciada(VendaIniciadaEvent e, CancellationToken cancellationToken)
    {
        var data = e.DataInicio.Date;
        var relatorio = await GetOrCreateRelatorio(data, cancellationToken);

        relatorio.TotalVendas++;
        relatorio.VendasEmAndamento++;
        relatorio.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleVendaFinalizada(VendaFinalizadaEvent e, CancellationToken cancellationToken)
    {
        var data = e.DataFinalizacao.Date;
        var relatorio = await GetOrCreateRelatorio(data, cancellationToken);

        relatorio.VendasEmAndamento = Math.Max(0, relatorio.VendasEmAndamento - 1);
        relatorio.VendasFinalizadas++;
        relatorio.ValorTotal += e.TotalFinal;
        relatorio.TicketMedio = relatorio.VendasFinalizadas > 0 
            ? relatorio.ValorTotal / relatorio.VendasFinalizadas 
            : 0;
        relatorio.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleVendaCancelada(VendaCanceladaEvent e, CancellationToken cancellationToken)
    {
        var data = e.DataCancelamento.Date;
        var relatorio = await GetOrCreateRelatorio(data, cancellationToken);

        relatorio.VendasEmAndamento = Math.Max(0, relatorio.VendasEmAndamento - 1);
        relatorio.VendasCanceladas++;
        relatorio.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleDescontoAplicado(DescontoAplicadoEvent e, CancellationToken cancellationToken)
    {
        var data = e.OccurredAt.Date;
        var relatorio = await GetOrCreateRelatorio(data, cancellationToken);

        relatorio.TotalDescontos += e.ValorDesconto;
        relatorio.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<RelatorioVendasReadModel> GetOrCreateRelatorio(DateTime data, CancellationToken cancellationToken)
    {
        var relatorio = await _context.Set<RelatorioVendasReadModel>()
            .FirstOrDefaultAsync(r => r.Data == data, cancellationToken);

        if (relatorio == null)
        {
            relatorio = new RelatorioVendasReadModel
            {
                Data = data,
                TotalVendas = 0,
                VendasFinalizadas = 0,
                VendasCanceladas = 0,
                VendasEmAndamento = 0,
                ValorTotal = 0,
                TotalDescontos = 0,
                TicketMedio = 0,
                LastUpdated = DateTime.UtcNow
            };

            await _context.Set<RelatorioVendasReadModel>().AddAsync(relatorio, cancellationToken);
        }

        return relatorio;
    }
}
