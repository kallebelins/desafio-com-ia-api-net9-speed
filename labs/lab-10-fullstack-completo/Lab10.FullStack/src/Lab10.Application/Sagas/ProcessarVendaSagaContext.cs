using Lab10.Domain.Entities;

namespace Lab10.Application.Sagas;

/// <summary>
/// Contexto da Saga de processamento de venda
/// </summary>
public class ProcessarVendaSagaContext
{
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public List<Produto> Produtos { get; set; } = new();
    public List<ItemSolicitado> ItensSolicitados { get; set; } = new();
    
    // Dados criados durante a saga
    public Venda? Venda { get; set; }
    public List<ReservaEstoque> ReservasRealizadas { get; set; } = new();
    
    // Flags de controle
    public bool ClienteValidado { get; set; }
    public bool ProdutosValidados { get; set; }
    public bool EstoqueReservado { get; set; }
    public bool VendaCriada { get; set; }
}

public class ItemSolicitado
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
}

public class ReservaEstoque
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
}
