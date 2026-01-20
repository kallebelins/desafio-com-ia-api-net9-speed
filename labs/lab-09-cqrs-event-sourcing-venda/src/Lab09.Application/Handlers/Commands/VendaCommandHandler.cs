using Lab09.Application.Commands;
using Lab09.Application.DTOs;
using Lab09.Core.Aggregates;
using Lab09.Core.Exceptions;
using Lab09.Core.Interfaces;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab09.Application.Handlers.Commands;

/// <summary>
/// Handler para todos os commands de Venda
/// </summary>
public class VendaCommandHandler :
    IMediatorCommandHandler<IniciarVendaCommand, VendaDto>,
    IMediatorCommandHandler<AdicionarItemCommand, VendaDto>,
    IMediatorCommandHandler<RemoverItemCommand, VendaDto>,
    IMediatorCommandHandler<AplicarDescontoCommand, VendaDto>,
    IMediatorCommandHandler<FinalizarVendaCommand, VendaDto>,
    IMediatorCommandHandler<CancelarVendaCommand, VendaDto>
{
    private readonly IEventStore _eventStore;
    private readonly ISnapshotStore _snapshotStore;

    public VendaCommandHandler(IEventStore eventStore, ISnapshotStore snapshotStore)
    {
        _eventStore = eventStore;
        _snapshotStore = snapshotStore;
    }

    /// <summary>
    /// Inicia uma nova venda
    /// </summary>
    public async Task<VendaDto> Handle(IniciarVendaCommand request, CancellationToken cancellationToken)
    {
        // Criar novo aggregate
        var venda = VendaAggregate.Iniciar(request.ClienteId);

        // Salvar eventos
        await _eventStore.SaveEventsAsync(
            venda.Id,
            venda.GetUncommittedEvents(),
            0, // Versão inicial
            cancellationToken);

        venda.ClearUncommittedEvents();

        return MapToDto(venda);
    }

    /// <summary>
    /// Adiciona um item à venda
    /// </summary>
    public async Task<VendaDto> Handle(AdicionarItemCommand request, CancellationToken cancellationToken)
    {
        var venda = await LoadAggregateAsync(request.VendaId, cancellationToken);

        venda.AdicionarItem(
            request.ProdutoId,
            request.ProdutoNome,
            request.Quantidade,
            request.PrecoUnitario);

        await SaveAndSnapshotAsync(venda, cancellationToken);

        return MapToDto(venda);
    }

    /// <summary>
    /// Remove um item da venda
    /// </summary>
    public async Task<VendaDto> Handle(RemoverItemCommand request, CancellationToken cancellationToken)
    {
        var venda = await LoadAggregateAsync(request.VendaId, cancellationToken);

        venda.RemoverItem(request.ProdutoId);

        await SaveAndSnapshotAsync(venda, cancellationToken);

        return MapToDto(venda);
    }

    /// <summary>
    /// Aplica desconto à venda
    /// </summary>
    public async Task<VendaDto> Handle(AplicarDescontoCommand request, CancellationToken cancellationToken)
    {
        var venda = await LoadAggregateAsync(request.VendaId, cancellationToken);

        if (request.PercentualDesconto.HasValue)
        {
            venda.AplicarDescontoPercentual(request.PercentualDesconto.Value, request.Motivo);
        }
        else if (request.ValorDesconto.HasValue)
        {
            venda.AplicarDesconto(request.ValorDesconto.Value, request.Motivo);
        }
        else
        {
            throw new DomainException("É necessário informar o valor ou percentual do desconto.");
        }

        await SaveAndSnapshotAsync(venda, cancellationToken);

        return MapToDto(venda);
    }

    /// <summary>
    /// Finaliza a venda
    /// </summary>
    public async Task<VendaDto> Handle(FinalizarVendaCommand request, CancellationToken cancellationToken)
    {
        var venda = await LoadAggregateAsync(request.VendaId, cancellationToken);

        venda.Finalizar();

        await SaveAndSnapshotAsync(venda, cancellationToken);

        return MapToDto(venda);
    }

    /// <summary>
    /// Cancela a venda
    /// </summary>
    public async Task<VendaDto> Handle(CancelarVendaCommand request, CancellationToken cancellationToken)
    {
        var venda = await LoadAggregateAsync(request.VendaId, cancellationToken);

        venda.Cancelar(request.Motivo);

        await SaveAndSnapshotAsync(venda, cancellationToken);

        return MapToDto(venda);
    }

    /// <summary>
    /// Carrega o Aggregate do Event Store (com otimização de Snapshot)
    /// </summary>
    private async Task<VendaAggregate> LoadAggregateAsync(Guid vendaId, CancellationToken cancellationToken)
    {
        // Tentar carregar do snapshot primeiro
        var snapshot = await _snapshotStore.GetSnapshotAsync<VendaAggregate>(vendaId, cancellationToken);

        if (snapshot != null)
        {
            // Carregar apenas eventos após o snapshot
            var newEvents = await _eventStore.GetEventsFromVersionAsync(
                vendaId,
                snapshot.Version,
                cancellationToken);

            foreach (var @event in newEvents)
            {
                snapshot.ApplyEvent(@event);
            }

            return snapshot;
        }

        // Sem snapshot, carregar todos os eventos
        var events = await _eventStore.GetEventsAsync(vendaId, cancellationToken);

        if (!events.Any())
            throw new DomainException($"Venda {vendaId} não encontrada.");

        return VendaAggregate.FromHistory(events);
    }

    /// <summary>
    /// Salva os eventos e cria snapshot se necessário
    /// </summary>
    private async Task SaveAndSnapshotAsync(VendaAggregate venda, CancellationToken cancellationToken)
    {
        var uncommittedEvents = venda.GetUncommittedEvents();
        var expectedVersion = venda.Version - uncommittedEvents.Count;

        await _eventStore.SaveEventsAsync(
            venda.Id,
            uncommittedEvents,
            expectedVersion,
            cancellationToken);

        venda.ClearUncommittedEvents();

        // Criar snapshot a cada 10 eventos
        if (venda.Version % 10 == 0)
        {
            await _snapshotStore.SaveSnapshotAsync(venda, cancellationToken);
        }
    }

    /// <summary>
    /// Mapeia o Aggregate para DTO
    /// </summary>
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
