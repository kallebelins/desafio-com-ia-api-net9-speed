using Mvp24Hours.Core.Entities;
using Lab10.Domain.ValueObjects;
using Lab10.Domain.Enums;
using Lab10.Domain.Exceptions;

namespace Lab10.Domain.Entities;

/// <summary>
/// Entidade Venda - Aggregate Root do processo de venda
/// </summary>
public class Venda : EntityBase<int>
{
    // Construtor protegido para EF Core
    protected Venda() { }

    public Venda(int clienteId)
    {
        ClienteId = clienteId;
        Status = VendaStatus.Pendente;
        DataCriacao = DateTime.UtcNow;
        _itens = new List<ItemVenda>();
    }

    public int ClienteId { get; private set; }
    public VendaStatus Status { get; private set; }
    public Money ValorTotal { get; private set; } = Money.Zero();
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }
    public DateTime? DataFinalizacao { get; private set; }
    public string? Observacao { get; private set; }

    // Navigation properties
    public Cliente? Cliente { get; private set; }
    public Pagamento? Pagamento { get; private set; }

    private readonly List<ItemVenda> _itens = new();
    public IReadOnlyCollection<ItemVenda> Itens => _itens.AsReadOnly();

    public ItemVenda AdicionarItem(int produtoId, string produtoNome, int quantidade, Money precoUnitario)
    {
        if (Status != VendaStatus.Pendente)
            throw new DomainException("Só é possível adicionar itens em vendas pendentes");

        if (quantidade <= 0)
            throw new DomainException("Quantidade deve ser maior que zero");

        var itemExistente = _itens.FirstOrDefault(i => i.ProdutoId == produtoId);
        if (itemExistente != null)
        {
            itemExistente.AtualizarQuantidade(itemExistente.Quantidade + quantidade);
            RecalcularTotal();
            return itemExistente;
        }

        var item = new ItemVenda(Id, produtoId, produtoNome, quantidade, precoUnitario);
        _itens.Add(item);
        RecalcularTotal();
        return item;
    }

    public void RemoverItem(int produtoId)
    {
        if (Status != VendaStatus.Pendente)
            throw new DomainException("Só é possível remover itens de vendas pendentes");

        var item = _itens.FirstOrDefault(i => i.ProdutoId == produtoId);
        if (item == null)
            throw new DomainException("Item não encontrado na venda");

        _itens.Remove(item);
        RecalcularTotal();
    }

    private void RecalcularTotal()
    {
        var total = _itens.Sum(i => i.ValorTotal.Valor);
        ValorTotal = Money.Create(total);
        DataAtualizacao = DateTime.UtcNow;
    }

    public void ConfirmarVenda()
    {
        if (Status != VendaStatus.Pendente)
            throw new DomainException("Só é possível confirmar vendas pendentes");

        if (!_itens.Any())
            throw new DomainException("Venda deve ter pelo menos um item");

        Status = VendaStatus.Confirmada;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void IniciarProcessamento()
    {
        if (Status != VendaStatus.Pendente && Status != VendaStatus.Confirmada)
            throw new DomainException("Só é possível processar vendas pendentes ou confirmadas");

        Status = VendaStatus.EmProcessamento;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AguardarPagamento()
    {
        if (Status != VendaStatus.EmProcessamento)
            throw new DomainException("Só é possível aguardar pagamento de vendas em processamento");

        Status = VendaStatus.AguardandoPagamento;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Finalizar()
    {
        if (Status != VendaStatus.AguardandoPagamento && Status != VendaStatus.EmProcessamento)
            throw new DomainException("Só é possível finalizar vendas em processamento ou aguardando pagamento");

        Status = VendaStatus.Finalizada;
        DataFinalizacao = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Cancelar(string? motivo = null)
    {
        if (Status == VendaStatus.Finalizada)
            throw new DomainException("Não é possível cancelar vendas finalizadas");

        if (Status == VendaStatus.Cancelada)
            throw new DomainException("Venda já está cancelada");

        Status = VendaStatus.Cancelada;
        Observacao = motivo;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void DefinirObservacao(string? observacao)
    {
        Observacao = observacao;
        DataAtualizacao = DateTime.UtcNow;
    }
}
