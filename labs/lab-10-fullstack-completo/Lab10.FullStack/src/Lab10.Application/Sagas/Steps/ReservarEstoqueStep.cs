using Microsoft.Extensions.Logging;
using Lab10.Application.Interfaces;
using Lab10.Domain.Interfaces;

namespace Lab10.Application.Sagas.Steps;

/// <summary>
/// Step 3: Reserva o estoque dos produtos
/// </summary>
public class ReservarEstoqueStep : ISagaStep<ProcessarVendaSagaContext>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IUnitOfWorkApplication _unitOfWork;
    private readonly ILogger<ReservarEstoqueStep> _logger;

    public ReservarEstoqueStep(
        IProdutoRepository produtoRepository,
        IUnitOfWorkApplication unitOfWork,
        ILogger<ReservarEstoqueStep> logger)
    {
        _produtoRepository = produtoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public string Name => "ReservarEstoque";
    public int Order => 3;
    public bool CanCompensate => true;

    public async Task ExecuteAsync(ProcessarVendaSagaContext context, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Reservando estoque para {Count} itens", context.ItensSolicitados.Count);

        var produtosDict = context.Produtos.ToDictionary(p => p.Id);

        foreach (var item in context.ItensSolicitados)
        {
            var produto = produtosDict[item.ProdutoId];
            
            _logger.LogInformation("Reservando {Quantidade} unidades de {ProdutoNome}", item.Quantidade, produto.Nome);
            
            produto.ReservarEstoque(item.Quantidade);
            await _produtoRepository.UpdateAsync(produto, cancellationToken);

            context.ReservasRealizadas.Add(new ReservaEstoque
            {
                ProdutoId = item.ProdutoId,
                Quantidade = item.Quantidade
            });
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        context.EstoqueReservado = true;

        _logger.LogInformation("Estoque reservado com sucesso para {Count} produtos", context.ReservasRealizadas.Count);
    }

    public async Task CompensateAsync(ProcessarVendaSagaContext context, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Compensando reservas de estoque. Reservas: {Count}", context.ReservasRealizadas.Count);

        foreach (var reserva in context.ReservasRealizadas)
        {
            try
            {
                var produto = await _produtoRepository.GetByIdAsync(reserva.ProdutoId, cancellationToken);
                if (produto != null)
                {
                    _logger.LogInformation("Liberando {Quantidade} unidades de {ProdutoNome}", reserva.Quantidade, produto.Nome);
                    produto.LiberarReserva(reserva.Quantidade);
                    await _produtoRepository.UpdateAsync(produto, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao liberar reserva do produto {ProdutoId}", reserva.ProdutoId);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        context.ReservasRealizadas.Clear();
        context.EstoqueReservado = false;

        _logger.LogInformation("Reservas de estoque compensadas com sucesso");
    }
}
