using System.Text.Json;
using Lab09.Core.Events;
using Lab09.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lab09.Application.Projections;

/// <summary>
/// Projeção que mantém o VendaReadModel atualizado
/// </summary>
public class VendaProjection : IProjection
{
    private readonly DbContext _context;

    public string Name => "VendaProjection";

    public VendaProjection(DbContext context)
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
            case ItemAdicionadoEvent e:
                await HandleItemAdicionado(e, cancellationToken);
                break;
            case ItemRemovidoEvent e:
                await HandleItemRemovido(e, cancellationToken);
                break;
            case DescontoAplicadoEvent e:
                await HandleDescontoAplicado(e, cancellationToken);
                break;
            case VendaFinalizadaEvent e:
                await HandleVendaFinalizada(e, cancellationToken);
                break;
            case VendaCanceladaEvent e:
                await HandleVendaCancelada(e, cancellationToken);
                break;
        }
    }

    private async Task HandleVendaIniciada(VendaIniciadaEvent e, CancellationToken cancellationToken)
    {
        var readModel = new VendaReadModel
        {
            Id = e.VendaId,
            ClienteId = e.ClienteId,
            DataInicio = e.DataInicio,
            Status = "EmAndamento",
            Subtotal = 0,
            Desconto = 0,
            Total = 0,
            QuantidadeItens = 0,
            ItensJson = "[]",
            Version = 1,
            LastUpdated = DateTime.UtcNow
        };

        await _context.Set<VendaReadModel>().AddAsync(readModel, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleItemAdicionado(ItemAdicionadoEvent e, CancellationToken cancellationToken)
    {
        var venda = await _context.Set<VendaReadModel>()
            .FirstOrDefaultAsync(v => v.Id == e.VendaId, cancellationToken);

        if (venda == null) return;

        var itens = JsonSerializer.Deserialize<List<ItemVendaReadModel>>(venda.ItensJson) ?? new List<ItemVendaReadModel>();
        
        itens.Add(new ItemVendaReadModel
        {
            ProdutoId = e.ProdutoId,
            ProdutoNome = e.ProdutoNome,
            Quantidade = e.Quantidade,
            PrecoUnitario = e.PrecoUnitario,
            Subtotal = e.Quantidade * e.PrecoUnitario
        });

        venda.ItensJson = JsonSerializer.Serialize(itens);
        venda.Subtotal = itens.Sum(i => i.Subtotal);
        venda.Total = venda.Subtotal - venda.Desconto;
        venda.QuantidadeItens = itens.Count;
        venda.Version++;
        venda.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleItemRemovido(ItemRemovidoEvent e, CancellationToken cancellationToken)
    {
        var venda = await _context.Set<VendaReadModel>()
            .FirstOrDefaultAsync(v => v.Id == e.VendaId, cancellationToken);

        if (venda == null) return;

        var itens = JsonSerializer.Deserialize<List<ItemVendaReadModel>>(venda.ItensJson) ?? new List<ItemVendaReadModel>();
        
        var itemToRemove = itens.FirstOrDefault(i => i.ProdutoId == e.ProdutoId);
        if (itemToRemove != null)
        {
            itens.Remove(itemToRemove);
        }

        venda.ItensJson = JsonSerializer.Serialize(itens);
        venda.Subtotal = itens.Sum(i => i.Subtotal);
        venda.Total = venda.Subtotal - venda.Desconto;
        venda.QuantidadeItens = itens.Count;
        venda.Version++;
        venda.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleDescontoAplicado(DescontoAplicadoEvent e, CancellationToken cancellationToken)
    {
        var venda = await _context.Set<VendaReadModel>()
            .FirstOrDefaultAsync(v => v.Id == e.VendaId, cancellationToken);

        if (venda == null) return;

        venda.Desconto = e.ValorDesconto;
        venda.Total = venda.Subtotal - venda.Desconto;
        venda.Version++;
        venda.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleVendaFinalizada(VendaFinalizadaEvent e, CancellationToken cancellationToken)
    {
        var venda = await _context.Set<VendaReadModel>()
            .FirstOrDefaultAsync(v => v.Id == e.VendaId, cancellationToken);

        if (venda == null) return;

        venda.Status = "Finalizada";
        venda.DataFinalizacao = e.DataFinalizacao;
        venda.Total = e.TotalFinal;
        venda.Version++;
        venda.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleVendaCancelada(VendaCanceladaEvent e, CancellationToken cancellationToken)
    {
        var venda = await _context.Set<VendaReadModel>()
            .FirstOrDefaultAsync(v => v.Id == e.VendaId, cancellationToken);

        if (venda == null) return;

        venda.Status = "Cancelada";
        venda.DataCancelamento = e.DataCancelamento;
        venda.Version++;
        venda.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
