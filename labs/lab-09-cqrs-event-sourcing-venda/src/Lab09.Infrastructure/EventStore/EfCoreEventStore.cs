using System.Text.Json;
using Lab09.Core.Events;
using Lab09.Core.Exceptions;
using Lab09.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lab09.Infrastructure.EventStore;

/// <summary>
/// Implementação do Event Store usando EF Core
/// </summary>
public class EfCoreEventStore : IEventStore
{
    private readonly EventStoreDbContext _context;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    // Mapeamento de tipos de eventos
    private static readonly Dictionary<string, Type> EventTypeMap = new()
    {
        { nameof(VendaIniciadaEvent), typeof(VendaIniciadaEvent) },
        { nameof(ItemAdicionadoEvent), typeof(ItemAdicionadoEvent) },
        { nameof(ItemRemovidoEvent), typeof(ItemRemovidoEvent) },
        { nameof(DescontoAplicadoEvent), typeof(DescontoAplicadoEvent) },
        { nameof(VendaFinalizadaEvent), typeof(VendaFinalizadaEvent) },
        { nameof(VendaCanceladaEvent), typeof(VendaCanceladaEvent) }
    };

    public EfCoreEventStore(EventStoreDbContext context)
    {
        _context = context;
    }

    public async Task SaveEventsAsync(
        Guid aggregateId,
        IEnumerable<IDomainEvent> events,
        int expectedVersion,
        CancellationToken cancellationToken = default)
    {
        var eventList = events.ToList();
        if (!eventList.Any()) return;

        // Verificar concorrência
        var currentVersion = await _context.StoredEvents
            .Where(e => e.AggregateId == aggregateId)
            .MaxAsync(e => (int?)e.Version, cancellationToken) ?? 0;

        if (currentVersion != expectedVersion)
        {
            throw new ConcurrencyException(aggregateId, expectedVersion, currentVersion);
        }

        var version = expectedVersion;
        foreach (var @event in eventList)
        {
            version++;
            var stored = new StoredEvent
            {
                EventId = @event.EventId,
                AggregateId = aggregateId,
                AggregateType = "VendaAggregate",
                EventType = @event.GetType().Name,
                EventData = JsonSerializer.Serialize(@event, @event.GetType(), JsonOptions),
                Version = version,
                Timestamp = @event.OccurredAt,
                Metadata = JsonSerializer.Serialize(new
                {
                    EventVersion = @event.EventVersion,
                    SavedAt = DateTime.UtcNow
                })
            };

            await _context.StoredEvents.AddAsync(stored, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IList<IDomainEvent>> GetEventsAsync(
        Guid aggregateId,
        CancellationToken cancellationToken = default)
    {
        var storedEvents = await _context.StoredEvents
            .Where(e => e.AggregateId == aggregateId)
            .OrderBy(e => e.Version)
            .ToListAsync(cancellationToken);

        return storedEvents
            .Select(DeserializeEvent)
            .ToList();
    }

    public async Task<IList<IDomainEvent>> GetEventsFromVersionAsync(
        Guid aggregateId,
        int fromVersion,
        CancellationToken cancellationToken = default)
    {
        var storedEvents = await _context.StoredEvents
            .Where(e => e.AggregateId == aggregateId && e.Version > fromVersion)
            .OrderBy(e => e.Version)
            .ToListAsync(cancellationToken);

        return storedEvents
            .Select(DeserializeEvent)
            .ToList();
    }

    public async Task<IList<(IDomainEvent Event, long Position)>> GetUnprocessedEventsAsync(
        long lastProcessedPosition,
        int batchSize = 100,
        CancellationToken cancellationToken = default)
    {
        var storedEvents = await _context.StoredEvents
            .Where(e => e.Id > lastProcessedPosition)
            .OrderBy(e => e.Id)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        return storedEvents
            .Select(e => (DeserializeEvent(e), e.Id))
            .ToList();
    }

    private IDomainEvent DeserializeEvent(StoredEvent stored)
    {
        if (!EventTypeMap.TryGetValue(stored.EventType, out var eventType))
        {
            throw new InvalidOperationException($"Tipo de evento desconhecido: {stored.EventType}");
        }

        var @event = JsonSerializer.Deserialize(stored.EventData, eventType, JsonOptions);
        
        if (@event is not IDomainEvent domainEvent)
        {
            throw new InvalidOperationException($"Falha ao deserializar evento: {stored.EventType}");
        }

        return domainEvent;
    }
}
