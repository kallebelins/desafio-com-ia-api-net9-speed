using Lab06.Application.Ports.Outbound;
using Lab06.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Lab06.Infrastructure.Adapters.Outbound.ExternalServices;

/// <summary>
/// Outbound Adapter - Implementação do serviço de validação de CPF externo
/// Em produção, usaria API da Receita Federal ou serviço similar
/// </summary>
public class CpfValidationService : ICpfValidationService
{
    private readonly ILogger<CpfValidationService> _logger;

    public CpfValidationService(ILogger<CpfValidationService> logger)
    {
        _logger = logger;
    }

    public Task<bool> ValidateAsync(string cpf, CancellationToken cancellationToken = default)
    {
        // Em produção, chamaria API externa (Receita Federal, Serpro, etc.)
        // Por enquanto, apenas valida o formato/algoritmo do CPF
        
        var isValid = CPF.IsValid(cpf);
        
        _logger.LogInformation(
            "🔍 [VALIDAÇÃO CPF SIMULADA] CPF: {Cpf} - Resultado: {Result}",
            MaskCpf(cpf),
            isValid ? "VÁLIDO" : "INVÁLIDO");

        return Task.FromResult(isValid);
    }

    public Task<CpfValidationResult> GetStatusAsync(string cpf, CancellationToken cancellationToken = default)
    {
        // Em produção, consultaria API externa para obter status completo
        var isValid = CPF.IsValid(cpf);

        _logger.LogInformation(
            "🔍 [CONSULTA STATUS CPF SIMULADA] CPF: {Cpf}",
            MaskCpf(cpf));

        if (!isValid)
        {
            return Task.FromResult(new CpfValidationResult(
                IsValid: false,
                IsActive: false,
                Name: null,
                StatusMessage: "CPF com formato inválido"
            ));
        }

        // Simulando uma resposta de sucesso
        return Task.FromResult(new CpfValidationResult(
            IsValid: true,
            IsActive: true,
            Name: "Nome Simulado",
            StatusMessage: "CPF regular - situação cadastral ativa"
        ));
    }

    private static string MaskCpf(string cpf)
    {
        var cpfNumbers = new string(cpf.Where(char.IsDigit).ToArray());
        if (cpfNumbers.Length != 11)
            return "***.***.***-**";
        
        return $"{cpfNumbers[..3]}.***.**{cpfNumbers.Substring(9, 2)}";
    }
}
