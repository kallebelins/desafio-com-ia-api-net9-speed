using Microsoft.Extensions.Logging;
using Lab10.Application.Interfaces;
using Lab10.Domain.Entities;
using Lab10.Domain.Interfaces;
using Lab10.Domain.Services;

namespace Lab10.Application.Sagas.Steps;

/// <summary>
/// Step 4: Cria a venda no banco de dados
/// </summary>
public class CriarVendaStep : ISagaStep<ProcessarVendaSagaContext>
{
    private readonly IVendaRepository _vendaRepository;
    private readonly IUnitOfWorkApplication _unitOfWork;
    private readonly VendaDomainService _vendaDomainService;
    private readonly ILogger<CriarVendaStep> _logger;

    public CriarVendaStep(
        IVendaRepository vendaRepository,
        IUnitOfWorkApplication unitOfWork,
        VendaDomainService vendaDomainService,
        ILogger<CriarVendaStep> logger)
    {
        _vendaRepository = vendaRepository;
        _unitOfWork = unitOfWork;
        _vendaDomainService = vendaDomainService;
        _logger = logger;
    }

    public string Name => "CriarVenda";
    public int Order => 4;
    public bool CanCompensate => true;

    public async Task ExecuteAsync(ProcessarVendaSagaContext context, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Criando venda para cliente: {ClienteId}", context.ClienteId);

        var produtosDict = context.Produtos.ToDictionary(p => p.Id);
        var itensParaVenda = context.ItensSolicitados
            .Select(i => (produtosDict[i.ProdutoId], i.Quantidade))
            .ToList();

        // Usar Domain Service para criar a venda
        var venda = _vendaDomainService.CriarVenda(context.Cliente!, itensParaVenda);

        // Confirmar a venda (mudar status para Confirmada)
        venda.ConfirmarVenda();

        await _vendaRepository.AddAsync(venda, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        context.Venda = venda;
        context.VendaCriada = true;

        _logger.LogInformation("Venda criada com sucesso. VendaId: {VendaId}, ValorTotal: {ValorTotal}", 
            venda.Id, venda.ValorTotal.Formatado);
    }

    public async Task CompensateAsync(ProcessarVendaSagaContext context, CancellationToken cancellationToken = default)
    {
        if (context.Venda == null)
        {
            _logger.LogInformation("Nenhuma venda para compensar");
            return;
        }

        _logger.LogInformation("Compensando criação da venda: {VendaId}", context.Venda.Id);

        try
        {
            context.Venda.Cancelar("Compensação de Saga");
            await _vendaRepository.UpdateAsync(context.Venda, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Venda cancelada com sucesso: {VendaId}", context.Venda.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao compensar venda: {VendaId}", context.Venda.Id);
        }

        context.VendaCriada = false;
    }
}
