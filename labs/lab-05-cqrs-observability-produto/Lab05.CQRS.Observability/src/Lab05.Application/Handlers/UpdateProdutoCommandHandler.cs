using Lab05.Application.Commands;
using Lab05.Application.Metrics;
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
/// Handler para o comando de atualização de produto
/// </summary>
public class UpdateProdutoCommandHandler : IMediatorCommandHandler<UpdateProdutoCommand, IBusinessResult<ProdutoDto>>
{
    private readonly IUnitOfWorkAsync _unitOfWork;
    private readonly ILogger<UpdateProdutoCommandHandler> _logger;
    private readonly ProdutoMetrics _metrics;

    public UpdateProdutoCommandHandler(
        IUnitOfWorkAsync unitOfWork,
        ILogger<UpdateProdutoCommandHandler> logger,
        ProdutoMetrics metrics)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _metrics = metrics;
    }

    public async Task<IBusinessResult<ProdutoDto>> Handle(UpdateProdutoCommand request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Atualizando produto Id: {Id}", request.Id);

            var repository = _unitOfWork.GetRepository<Produto>();
            var produto = await repository.GetByIdAsync(request.Id);

            if (produto == null)
            {
                _logger.LogWarning("Produto não encontrado. Id: {Id}", request.Id);
                return CreateErrorResult<ProdutoDto>($"Produto com Id '{request.Id}' não encontrado");
            }

            // Verificar se SKU já está em uso por outro produto
            var produtosComMesmoSKU = await repository.GetByAsync(p => p.SKU == request.SKU && p.Id != request.Id);
            if (produtosComMesmoSKU.Any())
            {
                _logger.LogWarning("SKU {SKU} já está em uso por outro produto", request.SKU);
                return CreateErrorResult<ProdutoDto>($"SKU '{request.SKU}' já está em uso por outro produto");
            }

            produto.Nome = request.Nome;
            produto.Descricao = request.Descricao;
            produto.Preco = request.Preco;
            produto.SKU = request.SKU;
            produto.Categoria = request.Categoria;
            produto.Ativo = request.Ativo;
            produto.DataAtualizacao = DateTime.UtcNow;

            await repository.ModifyAsync(produto);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _metrics.RecordProdutoAtualizado();

            var dto = MapToDto(produto);

            _logger.LogInformation("Produto atualizado com sucesso. Id: {Id}", produto.Id);

            return new BusinessResult<ProdutoDto>(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar produto Id: {Id}", request.Id);
            return CreateErrorResult<ProdutoDto>($"Erro ao atualizar produto: {ex.Message}");
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
