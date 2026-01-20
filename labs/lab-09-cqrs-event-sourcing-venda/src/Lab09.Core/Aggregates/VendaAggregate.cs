using Lab09.Core.Enums;
using Lab09.Core.Events;
using Lab09.Core.Exceptions;
using Lab09.Core.Interfaces;
using Lab09.Core.ValueObjects;

namespace Lab09.Core.Aggregates;

/// <summary>
/// Aggregate Root para Venda com Event Sourcing
/// O estado é derivado da sequência de eventos
/// </summary>
public class VendaAggregate : IAggregateRoot
{
    private readonly List<IDomainEvent> _uncommittedEvents = new();
    private readonly List<ItemVenda> _itens = new();

    // Propriedades de estado
    public Guid Id { get; private set; }
    public Guid ClienteId { get; private set; }
    public IReadOnlyList<ItemVenda> Itens => _itens.AsReadOnly();
    public decimal Subtotal { get; private set; }
    public decimal Desconto { get; private set; }
    public decimal Total => Subtotal - Desconto;
    public VendaStatus Status { get; private set; }
    public int Version { get; private set; }
    public DateTime? DataInicio { get; private set; }
    public DateTime? DataFinalizacao { get; private set; }
    public DateTime? DataCancelamento { get; private set; }

    // Construtor privado para uso interno
    private VendaAggregate() { }

    /// <summary>
    /// Reconstitui o Aggregate a partir do histórico de eventos
    /// </summary>
    public static VendaAggregate FromHistory(IEnumerable<IDomainEvent> events)
    {
        var aggregate = new VendaAggregate();
        foreach (var @event in events)
        {
            aggregate.ApplyEvent(@event);
            aggregate.Version++;
        }
        return aggregate;
    }

    /// <summary>
    /// Reconstitui o Aggregate a partir de um snapshot
    /// </summary>
    public static VendaAggregate FromSnapshot(object snapshotState, int version)
    {
        var aggregate = new VendaAggregate();
        
        // Usar reflexão ou casting para obter o estado
        dynamic state = snapshotState;
        
        aggregate.Id = state.Id;
        aggregate.ClienteId = state.ClienteId;
        aggregate.Subtotal = state.Subtotal;
        aggregate.Desconto = state.Desconto;
        aggregate.DataInicio = state.DataInicio;
        aggregate.DataFinalizacao = state.DataFinalizacao;
        aggregate.DataCancelamento = state.DataCancelamento;
        aggregate.Version = version;
        
        // Restaurar status
        string statusStr = state.Status;
        if (Enum.TryParse<VendaStatus>(statusStr, out VendaStatus parsedStatus))
        {
            aggregate.Status = parsedStatus;
        }
        
        // Restaurar itens
        foreach (var item in state.Itens)
        {
            aggregate._itens.Add(new ItemVenda(
                (Guid)item.ProdutoId,
                (string)item.ProdutoNome,
                (int)item.Quantidade,
                (decimal)item.PrecoUnitario));
        }
        
        return aggregate;
    }

    /// <summary>
    /// Cria uma nova venda (Factory Method)
    /// </summary>
    public static VendaAggregate Iniciar(Guid clienteId)
    {
        var aggregate = new VendaAggregate
        {
            Id = Guid.NewGuid()
        };

        aggregate.RaiseEvent(new VendaIniciadaEvent
        {
            VendaId = aggregate.Id,
            ClienteId = clienteId,
            DataInicio = DateTime.UtcNow
        });

        return aggregate;
    }

    /// <summary>
    /// Adiciona um item à venda
    /// </summary>
    public void AdicionarItem(Guid produtoId, string produtoNome, int quantidade, decimal precoUnitario)
    {
        if (Status != VendaStatus.EmAndamento)
            throw new DomainException("Não é possível adicionar itens. A venda não está em andamento.");

        if (quantidade <= 0)
            throw new DomainException("A quantidade deve ser maior que zero.");

        if (precoUnitario < 0)
            throw new DomainException("O preço unitário não pode ser negativo.");

        RaiseEvent(new ItemAdicionadoEvent
        {
            VendaId = Id,
            ProdutoId = produtoId,
            ProdutoNome = produtoNome,
            Quantidade = quantidade,
            PrecoUnitario = precoUnitario
        });
    }

    /// <summary>
    /// Remove um item da venda
    /// </summary>
    public void RemoverItem(Guid produtoId)
    {
        if (Status != VendaStatus.EmAndamento)
            throw new DomainException("Não é possível remover itens. A venda não está em andamento.");

        var item = _itens.FirstOrDefault(i => i.ProdutoId == produtoId);
        if (item == null)
            throw new DomainException($"Produto {produtoId} não encontrado na venda.");

        RaiseEvent(new ItemRemovidoEvent
        {
            VendaId = Id,
            ProdutoId = produtoId
        });
    }

    /// <summary>
    /// Aplica um desconto à venda
    /// </summary>
    public void AplicarDesconto(decimal valorDesconto, string? motivo = null)
    {
        if (Status != VendaStatus.EmAndamento)
            throw new DomainException("Não é possível aplicar desconto. A venda não está em andamento.");

        if (valorDesconto < 0)
            throw new DomainException("O valor do desconto não pode ser negativo.");

        if (valorDesconto > Subtotal)
            throw new DomainException("O valor do desconto não pode ser maior que o subtotal.");

        var percentual = Subtotal > 0 ? (valorDesconto / Subtotal) * 100 : 0;

        RaiseEvent(new DescontoAplicadoEvent
        {
            VendaId = Id,
            ValorDesconto = valorDesconto,
            PercentualDesconto = percentual,
            MotivoDesconto = motivo
        });
    }

    /// <summary>
    /// Aplica um desconto percentual à venda
    /// </summary>
    public void AplicarDescontoPercentual(decimal percentual, string? motivo = null)
    {
        if (percentual < 0 || percentual > 100)
            throw new DomainException("O percentual de desconto deve estar entre 0 e 100.");

        var valorDesconto = Subtotal * (percentual / 100);
        AplicarDesconto(valorDesconto, motivo);
    }

    /// <summary>
    /// Finaliza a venda
    /// </summary>
    public void Finalizar()
    {
        if (Status != VendaStatus.EmAndamento)
            throw new DomainException("Não é possível finalizar. A venda não está em andamento.");

        if (!_itens.Any())
            throw new DomainException("Não é possível finalizar uma venda sem itens.");

        RaiseEvent(new VendaFinalizadaEvent
        {
            VendaId = Id,
            TotalFinal = Total,
            DataFinalizacao = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Cancela a venda
    /// </summary>
    public void Cancelar(string motivo)
    {
        if (Status == VendaStatus.Cancelada)
            throw new DomainException("A venda já está cancelada.");

        if (Status == VendaStatus.Finalizada)
            throw new DomainException("Não é possível cancelar uma venda já finalizada.");

        if (string.IsNullOrWhiteSpace(motivo))
            throw new DomainException("É necessário informar o motivo do cancelamento.");

        RaiseEvent(new VendaCanceladaEvent
        {
            VendaId = Id,
            Motivo = motivo,
            DataCancelamento = DateTime.UtcNow
        });
    }

    // Implementação IAggregateRoot
    public IReadOnlyList<IDomainEvent> GetUncommittedEvents() => _uncommittedEvents.AsReadOnly();
    
    public void ClearUncommittedEvents() => _uncommittedEvents.Clear();

    /// <summary>
    /// Aplica um evento ao Aggregate, atualizando seu estado
    /// </summary>
    public void ApplyEvent(IDomainEvent @event)
    {
        switch (@event)
        {
            case VendaIniciadaEvent e:
                Apply(e);
                break;
            case ItemAdicionadoEvent e:
                Apply(e);
                break;
            case ItemRemovidoEvent e:
                Apply(e);
                break;
            case DescontoAplicadoEvent e:
                Apply(e);
                break;
            case VendaFinalizadaEvent e:
                Apply(e);
                break;
            case VendaCanceladaEvent e:
                Apply(e);
                break;
            default:
                throw new InvalidOperationException($"Evento desconhecido: {@event.GetType().Name}");
        }
    }

    // Métodos privados de aplicação de eventos
    private void Apply(VendaIniciadaEvent e)
    {
        Id = e.VendaId;
        ClienteId = e.ClienteId;
        DataInicio = e.DataInicio;
        Status = VendaStatus.EmAndamento;
    }

    private void Apply(ItemAdicionadoEvent e)
    {
        var item = new ItemVenda(e.ProdutoId, e.ProdutoNome, e.Quantidade, e.PrecoUnitario);
        _itens.Add(item);
        RecalcularSubtotal();
    }

    private void Apply(ItemRemovidoEvent e)
    {
        var item = _itens.FirstOrDefault(i => i.ProdutoId == e.ProdutoId);
        if (item != null)
        {
            _itens.Remove(item);
            RecalcularSubtotal();
        }
    }

    private void Apply(DescontoAplicadoEvent e)
    {
        Desconto = e.ValorDesconto;
    }

    private void Apply(VendaFinalizadaEvent e)
    {
        Status = VendaStatus.Finalizada;
        DataFinalizacao = e.DataFinalizacao;
    }

    private void Apply(VendaCanceladaEvent e)
    {
        Status = VendaStatus.Cancelada;
        DataCancelamento = e.DataCancelamento;
    }

    private void RecalcularSubtotal()
    {
        Subtotal = _itens.Sum(i => i.Subtotal);
    }

    /// <summary>
    /// Levanta um evento, aplicando-o ao estado e adicionando à lista de não confirmados
    /// </summary>
    private void RaiseEvent(IDomainEvent @event)
    {
        ApplyEvent(@event);
        _uncommittedEvents.Add(@event);
        Version++;
    }
}
