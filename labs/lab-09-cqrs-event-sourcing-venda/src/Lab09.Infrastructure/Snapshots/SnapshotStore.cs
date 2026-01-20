using System.Text.Json;
using Lab09.Core.Aggregates;
using Lab09.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lab09.Infrastructure.Snapshots;

/// <summary>
/// Implementação do Snapshot Store
/// </summary>
public class SnapshotStore : ISnapshotStore
{
    private readonly SnapshotDbContext _context;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public SnapshotStore(SnapshotDbContext context)
    {
        _context = context;
    }

    public async Task SaveSnapshotAsync<TAggregate>(TAggregate aggregate, CancellationToken cancellationToken = default)
        where TAggregate : IAggregateRoot
    {
        if (aggregate is VendaAggregate venda)
        {
            // Serializar o estado do aggregate
            var stateJson = SerializeVendaState(venda);

            var snapshot = new VendaSnapshot
            {
                Id = Guid.NewGuid(),
                AggregateId = venda.Id,
                Version = venda.Version,
                StateJson = stateJson,
                CreatedAt = DateTime.UtcNow
            };

            // Remover snapshots antigos do mesmo aggregate
            var oldSnapshots = await _context.VendaSnapshots
                .Where(s => s.AggregateId == venda.Id)
                .ToListAsync(cancellationToken);

            _context.VendaSnapshots.RemoveRange(oldSnapshots);

            await _context.VendaSnapshots.AddAsync(snapshot, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<TAggregate?> GetSnapshotAsync<TAggregate>(Guid aggregateId, CancellationToken cancellationToken = default)
        where TAggregate : IAggregateRoot
    {
        if (typeof(TAggregate) == typeof(VendaAggregate))
        {
            var snapshot = await _context.VendaSnapshots
                .Where(s => s.AggregateId == aggregateId)
                .OrderByDescending(s => s.Version)
                .FirstOrDefaultAsync(cancellationToken);

            if (snapshot == null)
                return default;

            var venda = DeserializeVendaState(snapshot.StateJson, snapshot.Version);
            return (TAggregate)(object)venda;
        }

        return default;
    }

    private string SerializeVendaState(VendaAggregate venda)
    {
        var state = new VendaSnapshotState
        {
            Id = venda.Id,
            ClienteId = venda.ClienteId,
            Itens = venda.Itens.Select(i => new ItemVendaSnapshotState
            {
                ProdutoId = i.ProdutoId,
                ProdutoNome = i.ProdutoNome,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario
            }).ToList(),
            Subtotal = venda.Subtotal,
            Desconto = venda.Desconto,
            Status = venda.Status.ToString(),
            DataInicio = venda.DataInicio,
            DataFinalizacao = venda.DataFinalizacao,
            DataCancelamento = venda.DataCancelamento
        };

        return JsonSerializer.Serialize(state, JsonOptions);
    }

    private VendaAggregate DeserializeVendaState(string json, int version)
    {
        var state = JsonSerializer.Deserialize<VendaSnapshotState>(json, JsonOptions)
            ?? throw new InvalidOperationException("Falha ao deserializar snapshot");

        // Reconstruir o aggregate a partir do estado
        // Nota: Isso requer que o aggregate tenha um método para restaurar do snapshot
        return VendaAggregate.FromSnapshot(state, version);
    }
}

/// <summary>
/// Estado serializado da Venda para snapshot
/// </summary>
public class VendaSnapshotState
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public List<ItemVendaSnapshotState> Itens { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal Desconto { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFinalizacao { get; set; }
    public DateTime? DataCancelamento { get; set; }
}

public class ItemVendaSnapshotState
{
    public Guid ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}
