using Lab07.Application.Services;
using Microsoft.Extensions.Logging;

namespace Lab07.Application.Sagas.Steps;

/// <summary>
/// Step 2: Validar se os produtos existem e têm estoque
/// </summary>
public class ValidarProdutosStep : ISagaStep<CriarVendaSagaContext>
{
    private readonly IProdutoService _produtoService;
    private readonly ILogger<ValidarProdutosStep> _logger;

    public string Name => "ValidarProdutos";
    public int Order => 2;
    public bool CanCompensate => false; // Apenas validação, nada a compensar

    public ValidarProdutosStep(
        IProdutoService produtoService,
        ILogger<ValidarProdutosStep> logger)
    {
        _produtoService = produtoService;
        _logger = logger;
    }

    public async Task ExecuteAsync(CriarVendaSagaContext data, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Validando {Count} produtos",
            data.Itens.Count);

        data.ProdutosValidados.Clear();

        foreach (var item in data.Itens)
        {
            var produto = await _produtoService.GetByIdAsync(item.ProdutoId, cancellationToken);

            if (produto == null)
            {
                data.SetError($"Produto {item.ProdutoId} não encontrado");
                return;
            }

            if (!produto.Ativo)
            {
                data.SetError($"Produto '{produto.Nome}' não está ativo");
                return;
            }

            if (produto.EstoqueDisponivel < item.Quantidade)
            {
                data.SetError(
                    $"Produto '{produto.Nome}' não tem estoque suficiente. " +
                    $"Disponível: {produto.EstoqueDisponivel}, Solicitado: {item.Quantidade}");
                return;
            }

            data.ProdutosValidados.Add(new ProdutoValidado
            {
                ProdutoId = produto.Id,
                Nome = produto.Nome,
                Preco = produto.Preco,
                QuantidadeSolicitada = item.Quantidade,
                EstoqueDisponivel = produto.EstoqueDisponivel
            });
        }

        // Calcula o valor total
        data.ValorTotal = data.ProdutosValidados
            .Sum(p => p.Preco * p.QuantidadeSolicitada);

        _logger.LogInformation(
            "Todos os {Count} produtos validados. Valor total: {ValorTotal:C}",
            data.ProdutosValidados.Count, data.ValorTotal);
    }

    public Task CompensateAsync(CriarVendaSagaContext data, CancellationToken cancellationToken = default)
    {
        // Nada a compensar - apenas validação
        return Task.CompletedTask;
    }
}
