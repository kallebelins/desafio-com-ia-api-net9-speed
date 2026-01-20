using Lab07.Core.Events;
using Lab07.Core.ValueObjects;

namespace Lab07.Application.Sagas;

/// <summary>
/// Contexto de dados para a saga de criação de venda
/// </summary>
public class CriarVendaSagaContext
{
    // Dados de entrada
    public Guid VendaId { get; set; }
    public Guid ClienteId { get; set; }
    public List<ItemVendaRequest> Itens { get; set; } = new();

    // Dados coletados durante a saga
    public string? ClienteNome { get; set; }
    public string? ClienteEmail { get; set; }
    public List<ProdutoValidado> ProdutosValidados { get; set; } = new();
    public List<ReservaEstoqueData> ReservasEstoque { get; set; } = new();
    public decimal ValorTotal { get; set; }

    // Resultado
    public VendaDto? VendaCriada { get; set; }
    public bool NotificacaoEnviada { get; set; }

    // Controle de erro
    public bool Failed { get; set; }
    public string? ErrorMessage { get; set; }

    public void SetError(string message)
    {
        Failed = true;
        ErrorMessage = message;
    }
}

/// <summary>
/// Dados de um produto validado
/// </summary>
public class ProdutoValidado
{
    public Guid ProdutoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int QuantidadeSolicitada { get; set; }
    public int EstoqueDisponivel { get; set; }
}
