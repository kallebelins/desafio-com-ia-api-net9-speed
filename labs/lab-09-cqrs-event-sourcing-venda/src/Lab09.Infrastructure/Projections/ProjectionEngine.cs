using Lab09.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lab09.Infrastructure.Projections;

/// <summary>
/// Engine para processar eventos e atualizar projeções
/// </summary>
public class ProjectionEngine
{
    private readonly IEventStore _eventStore;
    private readonly ProjectionDbContext _context;
    private readonly IEnumerable<IProjection> _projections;
    private readonly ILogger<ProjectionEngine> _logger;

    public ProjectionEngine(
        IEventStore eventStore,
        ProjectionDbContext context,
        IEnumerable<IProjection> projections,
        ILogger<ProjectionEngine> logger)
    {
        _eventStore = eventStore;
        _context = context;
        _projections = projections;
        _logger = logger;
    }

    /// <summary>
    /// Processa eventos pendentes
    /// </summary>
    public async Task ProcessPendingEventsAsync(CancellationToken cancellationToken = default)
    {
        // Obter último checkpoint
        var checkpoint = await _context.Checkpoints
            .FirstOrDefaultAsync(c => c.ProjectionName == "Global", cancellationToken);

        var lastPosition = checkpoint?.LastProcessedPosition ?? 0;

        // Buscar eventos não processados
        var events = await _eventStore.GetUnprocessedEventsAsync(lastPosition, 100, cancellationToken);

        if (!events.Any())
        {
            return;
        }

        _logger.LogInformation("Processando {Count} eventos a partir da posição {Position}",
            events.Count, lastPosition);

        foreach (var (evt, position) in events)
        {
            try
            {
                // Aplicar evento em todas as projeções
                foreach (var projection in _projections)
                {
                    await projection.HandleAsync(evt, cancellationToken);
                }

                // Atualizar checkpoint
                await UpdateCheckpointAsync(position, cancellationToken);

                _logger.LogDebug("Evento {EventType} na posição {Position} processado com sucesso",
                    evt.GetType().Name, position);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar evento na posição {Position}", position);
                throw;
            }
        }

        _logger.LogInformation("Processamento concluído. Nova posição: {Position}",
            events.Last().Position);
    }

    /// <summary>
    /// Reconstrói todas as projeções do zero
    /// </summary>
    public async Task RebuildProjectionsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Iniciando reconstrução de todas as projeções...");

        // Limpar dados existentes
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM VendasReadModel", cancellationToken);
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM RelatoriosVendasReadModel", cancellationToken);
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM ProjectionCheckpoints", cancellationToken);

        // Resetar checkpoint
        var checkpoint = new ProjectionCheckpoint
        {
            ProjectionName = "Global",
            LastProcessedPosition = 0,
            LastUpdated = DateTime.UtcNow
        };
        await _context.Checkpoints.AddAsync(checkpoint, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Reprocessar todos os eventos
        long lastPosition = 0;
        int totalProcessed = 0;

        while (true)
        {
            var events = await _eventStore.GetUnprocessedEventsAsync(lastPosition, 100, cancellationToken);
            
            if (!events.Any())
                break;

            foreach (var (evt, position) in events)
            {
                foreach (var projection in _projections)
                {
                    await projection.HandleAsync(evt, cancellationToken);
                }

                lastPosition = position;
                totalProcessed++;
            }

            await UpdateCheckpointAsync(lastPosition, cancellationToken);
        }

        _logger.LogInformation("Reconstrução concluída. Total de eventos processados: {Count}", totalProcessed);
    }

    private async Task UpdateCheckpointAsync(long position, CancellationToken cancellationToken)
    {
        var checkpoint = await _context.Checkpoints
            .FirstOrDefaultAsync(c => c.ProjectionName == "Global", cancellationToken);

        if (checkpoint == null)
        {
            checkpoint = new ProjectionCheckpoint
            {
                ProjectionName = "Global",
                LastProcessedPosition = position,
                LastUpdated = DateTime.UtcNow
            };
            await _context.Checkpoints.AddAsync(checkpoint, cancellationToken);
        }
        else
        {
            checkpoint.LastProcessedPosition = position;
            checkpoint.LastUpdated = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
