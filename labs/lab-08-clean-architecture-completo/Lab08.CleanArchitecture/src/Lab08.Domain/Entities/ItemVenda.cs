using Lab08.Domain.Exceptions;
using Lab08.Domain.ValueObjects;
using Mvp24Hours.Core.Entities;

namespace Lab08.Domain.Entities;

/// <summary>
/// Entidade Item de Venda
/// </summary>
public class ItemVenda : EntityBase<int>
{
    // Construtor para EF Core
    protected ItemVenda() { }

    internal ItemVenda(Venda venda, Produto produto, int quantidade)
    {
        Venda = venda;
        VendaId = venda.Id;
        Produto = produto;
        ProdutoId = produto.Id;
        Quantidade = quantidade;
        PrecoUnitario = produto.Preco;
        CalcularSubtotal();
    }

    public int Quantidade { get; private set; }
    public Money PrecoUnitario { get; private set; } = null!;
    public Money Subtotal { get; private set; } = null!;

    // Relacionamentos
    public int VendaId { get; private set; }
    public virtual Venda Venda { get; private set; } = null!;
    public int ProdutoId { get; private set; }
    public virtual Produto Produto { get; private set; } = null!;

    // Métodos de domínio
    internal void AtualizarQuantidade(int novaQuantidade)
    {
        if (novaQuantidade <= 0)
            throw new DomainException("Quantidade deve ser maior que zero");

        Quantidade = novaQuantidade;
        CalcularSubtotal();
    }

    private void CalcularSubtotal()
    {
        Subtotal = Money.Create(PrecoUnitario.Valor * Quantidade, PrecoUnitario.Moeda);
    }
}
