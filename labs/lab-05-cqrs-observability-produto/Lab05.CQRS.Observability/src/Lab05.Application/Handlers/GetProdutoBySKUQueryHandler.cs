using Lab05.Application.Metrics;
using Lab05.Application.Queries;
using Lab05.Core.Entities;
using Lab05.Core.ValueObjects;
using Microsoft.Extensions.Logging;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Core.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;
using System.Diagnostics;

namespace Lab05.Application.Handlers;

/// <summary>
/// Handler para a query de busca de produto por SKU
/// </summary>
public class GetProdutoBySKUQueryHandler : IMediatorQueryHandler<GetProdutoBySKUQuery, IBusinessResult<ProdutoDto>>
{
    private readonly IUnitOfWorkAsync _unitOfWork;
    private readonly ILogger<GetProdutoBySKUQueryHandler> _logger;
    private readonly ProdutoMetrics _metrics;

    public GetProdutoBySKUQueryHandler(
        IUnitOfWorkAsync unitOfWork,
        ILogger<GetProdutoBySKUQueryHandler> logger,
        ProdutoMetrics metrics)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _metrics = metrics;
    }

    public async Task<IBusinessResult<ProdutoDto>> Handle(GetProdutoBySKUQuery request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Buscando produto por SKU: {SKU}", request.SKU);

            var repository = _unitOfWork.GetRepository<Produto>();
            var produtos = await repository.GetByAsync(p => p.SKU == request.SKU);
            var produto = produtos.FirstOrDefault();

            if (produto == null)
            {
                _logger.LogWarning("Produto não encontrado. SKU: {SKU}", request.SKU);
                return CreateErrorResult<ProdutoDto>($"Produto com SKU '{request.SKU}' não encontrado");
            }

            var dto = MapToDto(produto);

            _logger.LogInformation("Produto encontrado. SKU: {SKU}, Nome: {Nome}", produto.SKU, produto.Nome);

            return new BusinessResult<ProdutoDto>(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produto por SKU: {SKU}", request.SKU);
            return CreateErrorResult<ProdutoDto>($"Erro ao buscar produto: {ex.Message}");
        }
        finally
        {
            stopwatch.Stop();
            _metrics.RecordDuration(stopwatch.Elapsed.TotalSeconds);
        }
    }

    private static ProdutoDto MapToDto(Produto produto)
    {
        return new ProdutoDto
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Preco = produto.Preco,
            SKU = produto.SKU,
            Categoria = produto.Categoria,
            Ativo = produto.Ativo,
            DataCriacao = produto.DataCriacao,
            DataAtualizacao = produto.DataAtualizacao
        };
    }

    private static IBusinessResult<T> CreateErrorResult<T>(string message)
    {
        IReadOnlyCollection<IMessageResult> messages = new List<IMessageResult>
        {
            new MessageResult(message, MessageType.Error)
        };
        return new BusinessResult<T>(default!, messages);
    }
}
