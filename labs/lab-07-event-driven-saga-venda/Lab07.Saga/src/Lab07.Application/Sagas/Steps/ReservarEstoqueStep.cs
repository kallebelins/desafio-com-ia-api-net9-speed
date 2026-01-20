using Lab07.Application.Services;
using Lab07.Core.Events;
using Microsoft.Extensions.Logging;

namespace Lab07.Application.Sagas.Steps;

/// <summary>
/// Step 3: Reservar estoque dos produtos
/// </summary>
public class ReservarEstoqueStep : ISagaStep<CriarVendaSagaContext>
{
    private readonly IProdutoService _produtoService;
    private readonly ILogger<ReservarEstoqueStep> _logger;

    public string Name => "ReservarEstoque";
    public int Order => 3;
    public bool CanCompensate => true;

    public ReservarEstoqueStep(
        IProdutoService produtoService,
        ILogger<ReservarEstoqueStep> logger)
    {
        _produtoService = produtoService;
        _logger = logger;
    }

    public async Task ExecuteAsync(CriarVendaSagaContext data, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Reservando estoque para {Count} produtos",
            data.ProdutosValidados.Count);

        data.ReservasEstoque.Clear();

        foreach (var produto in data.ProdutosValidados)
        {
            var sucesso = await _produtoService.ReservarEstoqueAsync(
                produto.ProdutoId,
                produto.QuantidadeSolicitada,
                cancellationToken);

            if (!sucesso)
            {
                data.SetError($"Falha ao reservar estoque do produto '{produto.Nome}'");
                return;
            }

            data.ReservasEstoque.Add(new ReservaEstoqueData
            {
                ProdutoId = produto.ProdutoId,
                Quantidade = produto.QuantidadeSolicitada
            });

            _logger.LogInformation(
                "Estoque reservado: Produto {ProdutoId}, Quantidade {Quantidade}",
                produto.ProdutoId, produto.QuantidadeSolicitada);
        }

        _logger.LogInformation(
            "Estoque reservado com sucesso para todos os {Count} produtos",
            data.ReservasEstoque.Count);
    }

    public async Task CompensateAsync(CriarVendaSagaContext data, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Compensando: Liberando {Count} reservas de estoque",
            data.ReservasEstoque.Count);

        foreach (var reserva in data.ReservasEstoque)
        {
            try
            {
                await _produtoService.LiberarReservaAsync(
                    reserva.ProdutoId,
                    reserva.Quantidade,
                    cancellationToken);

                _logger.LogInformation(
                    "Reserva liberada: Produto {ProdutoId}, Quantidade {Quantidade}",
                    reserva.ProdutoId, reserva.Quantidade);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Falha ao liberar reserva do produto {ProdutoId}",
                    reserva.ProdutoId);
            }
        }
    }
}
