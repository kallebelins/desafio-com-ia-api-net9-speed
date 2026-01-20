using Mvp24Hours.Core.Entities;
using Lab10.Domain.ValueObjects;
using Lab10.Domain.Exceptions;

namespace Lab10.Domain.Entities;

/// <summary>
/// Entidade Item de Venda
/// </summary>
public class ItemVenda : EntityBase<int>
{
    // Construtor protegido para EF Core
    protected ItemVenda() { }

    public ItemVenda(int vendaId, int produtoId, string produtoNome, int quantidade, Money precoUnitario)
    {
        VendaId = vendaId;
        ProdutoId = produtoId;
        ProdutoNome = produtoNome ?? throw new ArgumentNullException(nameof(produtoNome));
        Quantidade = quantidade > 0 ? quantidade : throw new DomainException("Quantidade deve ser maior que zero");
        PrecoUnitario = precoUnitario ?? throw new ArgumentNullException(nameof(precoUnitario));
    }

    public int VendaId { get; private set; }
    public int ProdutoId { get; private set; }
    public string ProdutoNome { get; private set; } = string.Empty;
    public int Quantidade { get; private set; }
    public Money PrecoUnitario { get; private set; } = null!;

    // Navigation properties
    public Venda? Venda { get; private set; }
    public Produto? Produto { get; private set; }

    /// <summary>
    /// Valor total do item = Quantidade * Preço Unitário
    /// </summary>
    public Money ValorTotal => Money.Create(Quantidade * PrecoUnitario.Valor);

    public void AtualizarQuantidade(int quantidade)
    {
        if (quantidade <= 0)
            throw new DomainException("Quantidade deve ser maior que zero");

        Quantidade = quantidade;
    }
}
