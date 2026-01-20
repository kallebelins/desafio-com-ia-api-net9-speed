namespace Lab09.Core.Enums;

/// <summary>
/// Status possíveis de uma Venda
/// </summary>
public enum VendaStatus
{
    /// <summary>
    /// Estado inicial (não iniciado)
    /// </summary>
    None = 0,
    
    /// <summary>
    /// Venda em andamento (pode adicionar/remover itens)
    /// </summary>
    EmAndamento = 1,
    
    /// <summary>
    /// Venda finalizada com sucesso
    /// </summary>
    Finalizada = 2,
    
    /// <summary>
    /// Venda cancelada
    /// </summary>
    Cancelada = 3
}
