using Lab08.Domain.Entities;
using Lab08.Domain.Exceptions;

namespace Lab08.Domain.Services;

/// <summary>
/// Domain Service para operações complexas de Venda que envolvem múltiplas entidades
/// </summary>
public class VendaDomainService
{
    /// <summary>
    /// Cria uma nova venda com múltiplos itens
    /// </summary>
    public Venda CriarVenda(Cliente cliente, IEnumerable<(Produto Produto, int Quantidade)> itens)
    {
        if (cliente == null)
            throw new DomainException("Cliente é obrigatório");

        if (!cliente.Ativo)
            throw new DomainException("Cliente está inativo e não pode realizar vendas");

        var listaItens = itens.ToList();
        if (!listaItens.Any())
            throw new DomainException("A venda deve ter pelo menos um item");

        var venda = new Venda(cliente.Id);

        foreach (var (produto, quantidade) in listaItens)
        {
            if (produto == null)
                throw new DomainException("Produto não encontrado");

            if (!produto.Ativo)
                throw new DomainException($"Produto '{produto.Nome}' está inativo");

            if (!produto.TemEstoqueDisponivel(quantidade))
                throw new DomainException($"Produto '{produto.Nome}' não possui estoque suficiente. Disponível: {produto.Estoque}");

            venda.AdicionarItem(produto, quantidade);
        }

        return venda;
    }

    /// <summary>
    /// Confirma uma venda e atualiza o estoque dos produtos
    /// </summary>
    public void ConfirmarVendaComBaixaEstoque(Venda venda, IEnumerable<Produto> produtos)
    {
        if (venda == null)
            throw new DomainException("Venda é obrigatória");

        var produtosDict = produtos.ToDictionary(p => p.Id);

        // Validar estoque antes de confirmar
        foreach (var item in venda.Itens)
        {
            if (!produtosDict.TryGetValue(item.ProdutoId, out var produto))
                throw new DomainException($"Produto com ID {item.ProdutoId} não encontrado");

            if (!produto.TemEstoqueDisponivel(item.Quantidade))
                throw new DomainException($"Produto '{produto.Nome}' não possui estoque suficiente");
        }

        // Confirmar venda
        venda.Confirmar();

        // Baixar estoque
        foreach (var item in venda.Itens)
        {
            var produto = produtosDict[item.ProdutoId];
            produto.RemoverEstoque(item.Quantidade);
        }
    }

    /// <summary>
    /// Cancela uma venda e estorna o estoque se necessário
    /// </summary>
    public void CancelarVendaComEstorno(Venda venda, IEnumerable<Produto> produtos, string? motivo = null)
    {
        if (venda == null)
            throw new DomainException("Venda é obrigatória");

        var produtosDict = produtos.ToDictionary(p => p.Id);
        var deveEstornarEstoque = venda.Status == Enums.StatusVenda.Confirmada;

        // Cancelar venda
        venda.Cancelar(motivo);

        // Estornar estoque se a venda já estava confirmada
        if (deveEstornarEstoque)
        {
            foreach (var item in venda.Itens)
            {
                if (produtosDict.TryGetValue(item.ProdutoId, out var produto))
                {
                    produto.AdicionarEstoque(item.Quantidade);
                }
            }
        }
    }
}
