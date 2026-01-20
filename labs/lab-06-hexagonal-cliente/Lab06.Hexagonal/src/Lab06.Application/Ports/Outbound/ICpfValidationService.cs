namespace Lab06.Application.Ports.Outbound;

/// <summary>
/// Outbound Port (Driven) - Interface para serviço externo de validação de CPF
/// </summary>
public interface ICpfValidationService
{
    /// <summary>
    /// Valida CPF com serviço externo (Receita Federal, etc.)
    /// </summary>
    /// <param name="cpf">CPF a ser validado (com ou sem formatação)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se o CPF é válido no serviço externo</returns>
    Task<bool> ValidateAsync(string cpf, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Verifica se o CPF está ativo e regular
    /// </summary>
    Task<CpfValidationResult> GetStatusAsync(string cpf, CancellationToken cancellationToken = default);
}

/// <summary>
/// Resultado da validação de CPF
/// </summary>
public record CpfValidationResult(
    bool IsValid,
    bool IsActive,
    string? Name,
    string? StatusMessage
);
