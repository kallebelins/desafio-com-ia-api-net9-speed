namespace Lab09.Application.Projections;

/// <summary>
/// Read Model para consultas otimizadas de vendas
/// </summary>
public class VendaReadModel
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Desconto { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public int QuantidadeItens { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFinalizacao { get; set; }
    public DateTime? DataCancelamento { get; set; }
    public int Version { get; set; }
    public DateTime LastUpdated { get; set; }
    
    // Itens em JSON para consultas simples
    public string ItensJson { get; set; } = "[]";
}

/// <summary>
/// Item do Read Model
/// </summary>
public class ItemVendaReadModel
{
    public Guid ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal Subtotal { get; set; }
}
