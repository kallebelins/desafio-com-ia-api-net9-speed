using Lab05.Application.Commands;
using Lab05.Application.Metrics;
using Lab05.Core.Entities;
using Microsoft.Extensions.Logging;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Core.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;
using System.Diagnostics;

namespace Lab05.Application.Handlers;

/// <summary>
/// Handler para o comando de exclusão de produto
/// </summary>
public class DeleteProdutoCommandHandler : IMediatorCommandHandler<DeleteProdutoCommand, IBusinessResult<bool>>
{
    private readonly IUnitOfWorkAsync _unitOfWork;
    private readonly ILogger<DeleteProdutoCommandHandler> _logger;
    private readonly ProdutoMetrics _metrics;

    public DeleteProdutoCommandHandler(
        IUnitOfWorkAsync unitOfWork,
        ILogger<DeleteProdutoCommandHandler> logger,
        ProdutoMetrics metrics)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _metrics = metrics;
    }

    public async Task<IBusinessResult<bool>> Handle(DeleteProdutoCommand request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Deletando produto Id: {Id}", request.Id);

            var repository = _unitOfWork.GetRepository<Produto>();
            var produto = await repository.GetByIdAsync(request.Id);

            if (produto == null)
            {
                _logger.LogWarning("Produto não encontrado para exclusão. Id: {Id}", request.Id);
                return CreateErrorResult<bool>($"Produto com Id '{request.Id}' não encontrado");
            }

            await repository.RemoveByIdAsync(request.Id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _metrics.RecordProdutoDeletado();

            _logger.LogInformation("Produto deletado com sucesso. Id: {Id}", request.Id);

            return new BusinessResult<bool>(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar produto Id: {Id}", request.Id);
            return CreateErrorResult<bool>($"Erro ao deletar produto: {ex.Message}");
        }
        finally
        {
            stopwatch.Stop();
            _metrics.RecordDuration(stopwatch.Elapsed.TotalSeconds);
        }
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
