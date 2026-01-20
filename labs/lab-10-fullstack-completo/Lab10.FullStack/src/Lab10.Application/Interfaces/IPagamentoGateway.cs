using Lab10.Domain.Enums;

namespace Lab10.Application.Interfaces;

/// <summary>
/// Interface do gateway de pagamento
/// </summary>
public interface IPagamentoGateway
{
    Task<PagamentoGatewayResult> ProcessarPagamentoAsync(
        int clienteId, 
        decimal valor, 
        MetodoPagamento metodo, 
        CancellationToken cancellationToken = default);

    Task<bool> EstornarPagamentoAsync(
        string transacaoId, 
        decimal valor, 
        CancellationToken cancellationToken = default);
}

public record PagamentoGatewayResult(
    bool Sucesso, 
    string? TransacaoId, 
    string? MotivoFalha);
