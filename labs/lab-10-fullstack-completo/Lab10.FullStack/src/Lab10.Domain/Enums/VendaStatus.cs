namespace Lab10.Domain.Enums;

/// <summary>
/// Status possíveis de uma Venda
/// </summary>
public enum VendaStatus
{
    /// <summary>
    /// Venda criada, aguardando itens e confirmação
    /// </summary>
    Pendente = 0,

    /// <summary>
    /// Venda confirmada pelo cliente
    /// </summary>
    Confirmada = 1,

    /// <summary>
    /// Venda em processamento (reserva de estoque, pagamento)
    /// </summary>
    EmProcessamento = 2,

    /// <summary>
    /// Aguardando confirmação de pagamento
    /// </summary>
    AguardandoPagamento = 3,

    /// <summary>
    /// Venda finalizada com sucesso
    /// </summary>
    Finalizada = 4,

    /// <summary>
    /// Venda cancelada
    /// </summary>
    Cancelada = 5
}
