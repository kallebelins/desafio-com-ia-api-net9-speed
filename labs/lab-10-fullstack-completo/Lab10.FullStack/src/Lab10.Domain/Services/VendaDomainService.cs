using Lab10.Domain.Entities;
using Lab10.Domain.ValueObjects;
using Lab10.Domain.Exceptions;

namespace Lab10.Domain.Services;

/// <summary>
/// Domain Service para operações complexas de Venda
/// </summary>
public class VendaDomainService
{
    /// <summary>
    /// Cria uma venda completa com validações de domínio
    /// </summary>
    public Venda CriarVenda(Cliente cliente, IEnumerable<(Produto produto, int quantidade)> itens)
    {
        if (cliente == null)
            throw new DomainException("Cliente é obrigatório");

        if (!cliente.Ativo)
            throw new DomainException("Cliente não está ativo");

        var listaItens = itens?.ToList() ?? throw new DomainException("Itens são obrigatórios");

        if (!listaItens.Any())
            throw new DomainException("A venda deve ter pelo menos um item");

        var venda = new Venda(cliente.Id);

        foreach (var (produto, quantidade) in listaItens)
        {
            if (!produto.Ativo)
                throw new DomainException($"Produto '{produto.Nome}' não está ativo");

            if (produto.EstoqueDisponivel < quantidade)
                throw new DomainException($"Estoque insuficiente para '{produto.Nome}'. Disponível: {produto.EstoqueDisponivel}");

            venda.AdicionarItem(produto.Id, produto.Nome, quantidade, produto.PrecoUnitario);
        }

        return venda;
    }

    /// <summary>
    /// Valida se a venda pode ser processada
    /// </summary>
    public void ValidarVendaParaProcessamento(Venda venda, Cliente cliente, IEnumerable<Produto> produtos)
    {
        if (venda == null)
            throw new DomainException("Venda não encontrada");

        if (cliente == null)
            throw new DomainException("Cliente não encontrado");

        if (!cliente.Ativo)
            throw new DomainException("Cliente não está ativo");

        var produtosDict = produtos.ToDictionary(p => p.Id);

        foreach (var item in venda.Itens)
        {
            if (!produtosDict.TryGetValue(item.ProdutoId, out var produto))
                throw new DomainException($"Produto {item.ProdutoId} não encontrado");

            if (!produto.Ativo)
                throw new DomainException($"Produto '{produto.Nome}' não está ativo");

            if (produto.EstoqueDisponivel < item.Quantidade)
                throw new DomainException($"Estoque insuficiente para '{produto.Nome}'");
        }
    }

    /// <summary>
    /// Calcula o valor total da venda com desconto (se aplicável)
    /// </summary>
    public Money CalcularValorComDesconto(Venda venda, decimal percentualDesconto)
    {
        if (percentualDesconto < 0 || percentualDesconto > 100)
            throw new DomainException("Percentual de desconto deve estar entre 0 e 100");

        var fatorDesconto = 1 - (percentualDesconto / 100);
        return venda.ValorTotal.Multiply(fatorDesconto);
    }
}
