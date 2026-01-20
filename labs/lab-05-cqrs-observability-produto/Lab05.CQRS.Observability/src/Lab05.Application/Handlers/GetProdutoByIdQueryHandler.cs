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
/// Handler para a query de busca de produto por ID
/// </summary>
public class GetProdutoByIdQueryHandler : IMediatorQueryHandler<GetProdutoByIdQuery, IBusinessResult<ProdutoDto>>
{
    private readonly IUnitOfWorkAsync _unitOfWork;
    private readonly ILogger<GetProdutoByIdQueryHandler> _logger;
    private readonly ProdutoMetrics _metrics;

    public GetProdutoByIdQueryHandler(
        IUnitOfWorkAsync unitOfWork,
        ILogger<GetProdutoByIdQueryHandler> logger,
        ProdutoMetrics metrics)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _metrics = metrics;
    }

    public async Task<IBusinessResult<ProdutoDto>> Handle(GetProdutoByIdQuery request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Buscando produto por Id: {Id}", request.Id);

            var repository = _unitOfWork.GetRepository<Produto>();
            var produto = await repository.GetByIdAsync(request.Id);

            if (produto == null)
            {
                _logger.LogWarning("Produto não encontrado. Id: {Id}", request.Id);
                return CreateErrorResult<ProdutoDto>($"Produto com Id '{request.Id}' não encontrado");
            }

            var dto = MapToDto(produto);

            _logger.LogInformation("Produto encontrado. Id: {Id}, Nome: {Nome}", produto.Id, produto.Nome);

            return new BusinessResult<ProdutoDto>(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produto por Id: {Id}", request.Id);
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
