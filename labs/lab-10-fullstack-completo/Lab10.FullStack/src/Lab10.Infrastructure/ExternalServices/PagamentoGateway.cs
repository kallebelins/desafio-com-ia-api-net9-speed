using Microsoft.Extensions.Logging;
using Lab10.Application.Interfaces;
using Lab10.Domain.Enums;

namespace Lab10.Infrastructure.ExternalServices;

/// <summary>
/// Implementação simulada do gateway de pagamento
/// </summary>
public class PagamentoGateway : IPagamentoGateway
{
    private readonly ILogger<PagamentoGateway> _logger;
    private readonly Random _random = new();

    public PagamentoGateway(ILogger<PagamentoGateway> logger)
    {
        _logger = logger;
    }

    public async Task<PagamentoGatewayResult> ProcessarPagamentoAsync(
        int clienteId, 
        decimal valor, 
        MetodoPagamento metodo, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[PAYMENT GATEWAY] Processando pagamento: ClienteId={ClienteId}, Valor={Valor}, Metodo={Metodo}",
            clienteId, valor, metodo);

        // Simular processamento
        await Task.Delay(500, cancellationToken);

        // Simular taxa de sucesso de 90%
        var sucesso = _random.Next(100) < 90;

        if (sucesso)
        {
            var transacaoId = $"TXN-{Guid.NewGuid():N}"[..20];
            _logger.LogInformation(
                "[PAYMENT GATEWAY] Pagamento aprovado: TransacaoId={TransacaoId}",
                transacaoId);

            return new PagamentoGatewayResult(true, transacaoId, null);
        }
        else
        {
            var motivos = new[]
            {
                "Cartão recusado",
                "Saldo insuficiente",
                "Transação não autorizada",
                "Timeout na operação"
            };

            var motivo = motivos[_random.Next(motivos.Length)];
            _logger.LogWarning("[PAYMENT GATEWAY] Pagamento rejeitado: {Motivo}", motivo);

            return new PagamentoGatewayResult(false, null, motivo);
        }
    }

    public async Task<bool> EstornarPagamentoAsync(
        string transacaoId, 
        decimal valor, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[PAYMENT GATEWAY] Estornando pagamento: TransacaoId={TransacaoId}, Valor={Valor}",
            transacaoId, valor);

        // Simular processamento
        await Task.Delay(300, cancellationToken);

        _logger.LogInformation("[PAYMENT GATEWAY] Estorno realizado com sucesso");
        return true;
    }
}
