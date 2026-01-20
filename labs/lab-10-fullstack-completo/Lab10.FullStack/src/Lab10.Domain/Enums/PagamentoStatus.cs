namespace Lab10.Domain.Enums;

/// <summary>
/// Status possíveis de um Pagamento
/// </summary>
public enum PagamentoStatus
{
    /// <summary>
    /// Pagamento criado, aguardando processamento
    /// </summary>
    Pendente = 0,

    /// <summary>
    /// Pagamento em processamento pelo gateway
    /// </summary>
    Processando = 1,

    /// <summary>
    /// Pagamento aprovado
    /// </summary>
    Aprovado = 2,

    /// <summary>
    /// Pagamento rejeitado pelo gateway
    /// </summary>
    Rejeitado = 3,

    /// <summary>
    /// Pagamento estornado
    /// </summary>
    Estornado = 4
}
