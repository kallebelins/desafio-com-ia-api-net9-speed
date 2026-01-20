namespace Lab10.Domain.Enums;

/// <summary>
/// Métodos de pagamento disponíveis
/// </summary>
public enum MetodoPagamento
{
    /// <summary>
    /// Cartão de Crédito
    /// </summary>
    CartaoCredito = 0,

    /// <summary>
    /// Cartão de Débito
    /// </summary>
    CartaoDebito = 1,

    /// <summary>
    /// PIX
    /// </summary>
    Pix = 2,

    /// <summary>
    /// Boleto Bancário
    /// </summary>
    Boleto = 3
}
