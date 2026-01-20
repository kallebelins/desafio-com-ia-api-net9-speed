using Lab08.Domain.Enums;
using Lab08.Domain.Exceptions;
using Lab08.Domain.ValueObjects;
using Mvp24Hours.Core.Entities;

namespace Lab08.Domain.Entities;

/// <summary>
/// Entidade Venda - Aggregate Root
/// </summary>
public class Venda : EntityBase<int>
{
    private readonly List<ItemVenda> _itens = new();

    // Construtor para EF Core
    protected Venda() { }

    public Venda(int clienteId)
    {
        ClienteId = clienteId;
        DataVenda = DateTime.UtcNow;
        Status = StatusVenda.Pendente;
        Total = Money.Zero("BRL");
    }

    public DateTime DataVenda { get; private set; }
    public StatusVenda Status { get; private set; }
    public Money Total { get; private set; } = null!;
    public string? Observacao { get; private set; }

    // Relacionamentos
    public int ClienteId { get; private set; }
    public virtual Cliente Cliente { get; private set; } = null!;
    public virtual IReadOnlyCollection<ItemVenda> Itens => _itens.AsReadOnly();

    // Métodos de domínio
    public ItemVenda AdicionarItem(Produto produto, int quantidade)
    {
        if (Status != StatusVenda.Pendente)
            throw new DomainException("Não é possível adicionar itens a uma venda que não está pendente");

        if (!produto.TemEstoqueDisponivel(quantidade))
            throw new DomainException($"Produto '{produto.Nome}' não possui estoque suficiente");

        var itemExistente = _itens.FirstOrDefault(i => i.ProdutoId == produto.Id);
        if (itemExistente != null)
        {
            itemExistente.AtualizarQuantidade(itemExistente.Quantidade + quantidade);
            RecalcularTotal();
            return itemExistente;
        }

        var item = new ItemVenda(this, produto, quantidade);
        _itens.Add(item);
        RecalcularTotal();

        return item;
    }

    public void RemoverItem(int produtoId)
    {
        if (Status != StatusVenda.Pendente)
            throw new DomainException("Não é possível remover itens de uma venda que não está pendente");

        var item = _itens.FirstOrDefault(i => i.ProdutoId == produtoId);
        if (item == null)
            throw new DomainException("Item não encontrado na venda");

        _itens.Remove(item);
        RecalcularTotal();
    }

    public void Confirmar()
    {
        if (Status != StatusVenda.Pendente)
            throw new DomainException("Apenas vendas pendentes podem ser confirmadas");

        if (!_itens.Any())
            throw new DomainException("A venda deve ter pelo menos um item");

        Status = StatusVenda.Confirmada;
    }

    public void Cancelar(string? motivo = null)
    {
        if (Status == StatusVenda.Cancelada)
            throw new DomainException("A venda já está cancelada");

        if (Status == StatusVenda.Entregue)
            throw new DomainException("Não é possível cancelar uma venda já entregue");

        Status = StatusVenda.Cancelada;
        Observacao = motivo;
    }

    public void MarcarComoEntregue()
    {
        if (Status != StatusVenda.Confirmada)
            throw new DomainException("Apenas vendas confirmadas podem ser marcadas como entregues");

        Status = StatusVenda.Entregue;
    }

    public void AdicionarObservacao(string observacao)
    {
        Observacao = observacao;
    }

    private void RecalcularTotal()
    {
        var total = _itens.Sum(i => i.Subtotal.Valor);
        Total = Money.Create(total, "BRL");
    }
}
