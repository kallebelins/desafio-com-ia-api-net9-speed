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
/// Handler para o comando de criação de produto
/// </summary>
public class CreateProdutoCommandHandler : IMediatorCommandHandler<CreateProdutoCommand, IBusinessResult<ProdutoDto>>
{
    private readonly IUnitOfWorkAsync _unitOfWork;
    private readonly ILogger<CreateProdutoCommandHandler> _logger;
    private readonly ProdutoMetrics _metrics;

    public CreateProdutoCommandHandler(
        IUnitOfWorkAsync unitOfWork,
        ILogger<CreateProdutoCommandHandler> logger,
        ProdutoMetrics metrics)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _metrics = metrics;
    }

    public async Task<IBusinessResult<ProdutoDto>> Handle(CreateProdutoCommand request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Criando produto com SKU: {SKU}", request.SKU);

            var repository = _unitOfWork.GetRepository<Produto>();

            // Verificar se já existe produto com o mesmo SKU
            var existingProducts = await repository.GetByAsync(p => p.SKU == request.SKU);
            if (existingProducts.Any())
            {
                _logger.LogWarning("Produto com SKU {SKU} já existe", request.SKU);
                return CreateErrorResult<ProdutoDto>($"Produto com SKU '{request.SKU}' já existe");
            }

            var produto = new Produto
            {
                Nome = request.Nome,
                Descricao = request.Descricao,
                Preco = request.Preco,
                SKU = request.SKU,
                Categoria = request.Categoria,
                Ativo = true
            };

            await repository.AddAsync(produto);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _metrics.RecordProdutoCriado();

            var dto = MapToDto(produto);

            _logger.LogInformation("Produto criado com sucesso. Id: {Id}, SKU: {SKU}", produto.Id, produto.SKU);

            return new BusinessResult<ProdutoDto>(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar produto com SKU: {SKU}", request.SKU);
            return CreateErrorResult<ProdutoDto>($"Erro ao criar produto: {ex.Message}");
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
