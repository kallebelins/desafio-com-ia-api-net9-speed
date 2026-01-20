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
/// Handler para a query de listagem de produtos
/// </summary>
public class GetProdutosQueryHandler : IMediatorQueryHandler<GetProdutosQuery, IBusinessResult<IList<ProdutoDto>>>
{
    private readonly IUnitOfWorkAsync _unitOfWork;
    private readonly ILogger<GetProdutosQueryHandler> _logger;
    private readonly ProdutoMetrics _metrics;

    public GetProdutosQueryHandler(
        IUnitOfWorkAsync unitOfWork,
        ILogger<GetProdutosQueryHandler> logger,
        ProdutoMetrics metrics)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _metrics = metrics;
    }

    public async Task<IBusinessResult<IList<ProdutoDto>>> Handle(GetProdutosQuery request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Listando produtos. ApenasAtivos: {ApenasAtivos}, Categoria: {Categoria}",
                request.ApenasAtivos, request.Categoria);

            var repository = _unitOfWork.GetRepository<Produto>();

            IList<Produto> produtos;

            if (request.ApenasAtivos.HasValue && request.ApenasAtivos.Value)
            {
                if (!string.IsNullOrEmpty(request.Categoria))
                {
                    produtos = await repository.GetByAsync(p => p.Ativo && p.Categoria == request.Categoria);
                }
                else
                {
                    produtos = await repository.GetByAsync(p => p.Ativo);
                }
            }
            else if (!string.IsNullOrEmpty(request.Categoria))
            {
                produtos = await repository.GetByAsync(p => p.Categoria == request.Categoria);
            }
            else
            {
                produtos = await repository.ListAsync();
            }

            var dtos = produtos.Select(MapToDto).ToList();

            _logger.LogInformation("Produtos encontrados: {Count}", dtos.Count);

            return new BusinessResult<IList<ProdutoDto>>(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar produtos");
            return CreateErrorResult<IList<ProdutoDto>>($"Erro ao listar produtos: {ex.Message}");
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
