using Microsoft.Extensions.Logging;
using Lab10.Domain.Exceptions;

namespace Lab10.Application.Sagas.Steps;

/// <summary>
/// Step 2: Valida se os produtos existem, estão ativos e tem estoque
/// </summary>
public class ValidarProdutosStep : ISagaStep<ProcessarVendaSagaContext>
{
    private readonly ILogger<ValidarProdutosStep> _logger;

    public ValidarProdutosStep(ILogger<ValidarProdutosStep> logger)
    {
        _logger = logger;
    }

    public string Name => "ValidarProdutos";
    public int Order => 2;
    public bool CanCompensate => false; // Validação não precisa compensar

    public Task ExecuteAsync(ProcessarVendaSagaContext context, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Validando produtos. Quantidade de itens: {Count}", context.ItensSolicitados.Count);

        if (!context.ItensSolicitados.Any())
            throw new DomainException("Nenhum item informado para a venda");

        var produtosDict = context.Produtos.ToDictionary(p => p.Id);

        foreach (var item in context.ItensSolicitados)
        {
            if (!produtosDict.TryGetValue(item.ProdutoId, out var produto))
                throw new DomainException($"Produto {item.ProdutoId} não encontrado");

            if (!produto.Ativo)
                throw new DomainException($"Produto '{produto.Nome}' não está ativo");

            if (produto.EstoqueDisponivel < item.Quantidade)
                throw new DomainException($"Estoque insuficiente para '{produto.Nome}'. Disponível: {produto.EstoqueDisponivel}, Solicitado: {item.Quantidade}");

            _logger.LogInformation("Produto validado: {ProdutoNome}, Quantidade: {Quantidade}", produto.Nome, item.Quantidade);
        }

        context.ProdutosValidados = true;
        _logger.LogInformation("Todos os produtos validados com sucesso");

        return Task.CompletedTask;
    }

    public Task CompensateAsync(ProcessarVendaSagaContext context, CancellationToken cancellationToken = default)
    {
        // Nada a compensar - apenas validação
        return Task.CompletedTask;
    }
}
