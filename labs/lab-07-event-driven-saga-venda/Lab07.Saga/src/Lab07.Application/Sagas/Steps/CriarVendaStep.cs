using Lab07.Application.Services;
using Lab07.Core.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Lab07.Application.Sagas.Steps;

/// <summary>
/// Step 4: Criar a venda no banco de dados
/// </summary>
public class CriarVendaStep : ISagaStep<CriarVendaSagaContext>
{
    private readonly IVendaService _vendaService;
    private readonly ILogger<CriarVendaStep> _logger;

    public string Name => "CriarVenda";
    public int Order => 4;
    public bool CanCompensate => true;

    public CriarVendaStep(
        IVendaService vendaService,
        ILogger<CriarVendaStep> logger)
    {
        _vendaService = vendaService;
        _logger = logger;
    }

    public async Task ExecuteAsync(CriarVendaSagaContext data, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Criando venda {VendaId} para cliente {ClienteId}",
            data.VendaId, data.ClienteId);

        // Prepara os itens da venda
        var itens = data.ProdutosValidados.Select(p => new ItemVendaDto
        {
            ProdutoId = p.ProdutoId,
            ProdutoNome = p.Nome,
            Quantidade = p.QuantidadeSolicitada,
            PrecoUnitario = p.Preco,
            ValorTotal = p.Preco * p.QuantidadeSolicitada
        }).ToList();

        // Cria a venda
        var venda = await _vendaService.CriarVendaAsync(
            data.VendaId,
            data.ClienteId,
            itens,
            data.ValorTotal,
            cancellationToken);

        if (venda == null)
        {
            data.SetError("Falha ao criar a venda no banco de dados");
            return;
        }

        data.VendaCriada = venda;

        _logger.LogInformation(
            "Venda {VendaId} criada com sucesso. Valor total: {ValorTotal:C}",
            data.VendaId, data.ValorTotal);
    }

    public async Task CompensateAsync(CriarVendaSagaContext data, CancellationToken cancellationToken = default)
    {
        if (data.VendaCriada == null)
        {
            _logger.LogWarning("Nenhuma venda para cancelar");
            return;
        }

        _logger.LogInformation(
            "Compensando: Cancelando venda {VendaId}",
            data.VendaId);

        try
        {
            await _vendaService.CancelarVendaAsync(
                data.VendaId,
                "Compensação de saga - falha em step posterior",
                cancellationToken);

            _logger.LogInformation(
                "Venda {VendaId} cancelada com sucesso",
                data.VendaId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Falha ao cancelar venda {VendaId}",
                data.VendaId);
        }
    }
}
