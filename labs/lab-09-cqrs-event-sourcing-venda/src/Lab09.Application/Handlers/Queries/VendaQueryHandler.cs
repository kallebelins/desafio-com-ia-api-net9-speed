using System.Text.Json;
using Lab09.Application.DTOs;
using Lab09.Application.Projections;
using Lab09.Application.Queries;
using Lab09.Core.Aggregates;
using Lab09.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab09.Application.Handlers.Queries;

/// <summary>
/// Handler para queries de Venda
/// </summary>
public class VendaQueryHandler :
    IMediatorQueryHandler<GetVendaByIdQuery, VendaDto?>,
    IMediatorQueryHandler<GetVendaHistoryQuery, VendaHistoryDto?>,
    IMediatorQueryHandler<GetVendaAtMomentQuery, VendaDto?>,
    IMediatorQueryHandler<GetVendasPorPeriodoQuery, IEnumerable<VendaReadModel>>
{
    private readonly IEventStore _eventStore;
    private readonly DbContext _readModelContext;

    public VendaQueryHandler(IEventStore eventStore, DbContext readModelContext)
    {
        _eventStore = eventStore;
        _readModelContext = readModelContext;
    }

    /// <summary>
    /// Obtém uma venda por ID (reconstrói do Event Store)
    /// </summary>
    public async Task<VendaDto?> Handle(GetVendaByIdQuery request, CancellationToken cancellationToken)
    {
        var events = await _eventStore.GetEventsAsync(request.VendaId, cancellationToken);

        if (!events.Any())
            return null;

        var venda = VendaAggregate.FromHistory(events);
        return MapToDto(venda);
    }

    /// <summary>
    /// Obtém o histórico completo de eventos de uma venda
    /// </summary>
    public async Task<VendaHistoryDto?> Handle(GetVendaHistoryQuery request, CancellationToken cancellationToken)
    {
        var events = await _eventStore.GetEventsAsync(request.VendaId, cancellationToken);

        if (!events.Any())
            return null;

        var version = 0;
        var eventDtos = events.Select(e => new EventoDto
        {
            EventId = e.EventId,
            TipoEvento = e.GetType().Name,
            OccurredAt = e.OccurredAt,
            Dados = e,
            Version = ++version
        }).ToList();

        return new VendaHistoryDto
        {
            VendaId = request.VendaId,
            Eventos = eventDtos,
            TotalEventos = eventDtos.Count
        };
    }

    /// <summary>
    /// Reconstrói o estado da venda em um momento específico (Time Travel)
    /// </summary>
    public async Task<VendaDto?> Handle(GetVendaAtMomentQuery request, CancellationToken cancellationToken)
    {
        var allEvents = await _eventStore.GetEventsAsync(request.VendaId, cancellationToken);

        if (!allEvents.Any())
            return null;

        // Filtrar eventos até o momento especificado
        var eventsUntilMoment = allEvents
            .Where(e => e.OccurredAt <= request.Momento)
            .ToList();

        if (!eventsUntilMoment.Any())
            return null;

        var venda = VendaAggregate.FromHistory(eventsUntilMoment);
        return MapToDto(venda);
    }

    /// <summary>
    /// Obtém vendas por período (consulta no Read Model)
    /// </summary>
    public async Task<IEnumerable<VendaReadModel>> Handle(GetVendasPorPeriodoQuery request, CancellationToken cancellationToken)
    {
        var query = _readModelContext.Set<VendaReadModel>().AsQueryable();

        if (request.DataInicio.HasValue)
            query = query.Where(v => v.DataInicio >= request.DataInicio.Value);

        if (request.DataFim.HasValue)
            query = query.Where(v => v.DataInicio <= request.DataFim.Value);

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(v => v.Status == request.Status);

        return await query
            .OrderByDescending(v => v.DataInicio)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
    }

    private static VendaDto MapToDto(VendaAggregate venda)
    {
        return new VendaDto
        {
            Id = venda.Id,
            ClienteId = venda.ClienteId,
            Itens = venda.Itens.Select(i => new ItemVendaDto
            {
                ProdutoId = i.ProdutoId,
                ProdutoNome = i.ProdutoNome,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario,
                Subtotal = i.Subtotal
            }).ToList(),
            Subtotal = venda.Subtotal,
            Desconto = venda.Desconto,
            Total = venda.Total,
            Status = venda.Status.ToString(),
            DataInicio = venda.DataInicio,
            DataFinalizacao = venda.DataFinalizacao,
            DataCancelamento = venda.DataCancelamento,
            Version = venda.Version
        };
    }
}
