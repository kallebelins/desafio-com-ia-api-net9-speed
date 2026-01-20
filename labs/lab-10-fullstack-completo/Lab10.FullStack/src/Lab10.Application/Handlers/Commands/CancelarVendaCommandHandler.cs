using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Core.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;
using Microsoft.Extensions.Logging;
using Lab10.Application.Commands.Vendas;
using Lab10.Application.Interfaces;
using Lab10.Domain.Enums;
using Lab10.Domain.Interfaces;

namespace Lab10.Application.Handlers.Commands;

public class CancelarVendaCommandHandler : IMediatorCommandHandler<CancelarVendaCommand, IBusinessResult<bool>>
{
    private readonly IVendaRepository _vendaRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IUnitOfWorkApplication _unitOfWork;
    private readonly ILogger<CancelarVendaCommandHandler> _logger;

    public CancelarVendaCommandHandler(
        IVendaRepository vendaRepository,
        IProdutoRepository produtoRepository,
        IUnitOfWorkApplication unitOfWork,
        ILogger<CancelarVendaCommandHandler> logger)
    {
        _vendaRepository = vendaRepository;
        _produtoRepository = produtoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IBusinessResult<bool>> Handle(CancelarVendaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Cancelando venda: {VendaId}, Motivo: {Motivo}", request.VendaId, request.Motivo);

            var venda = await _vendaRepository.GetByIdWithItensAsync(request.VendaId, cancellationToken);
            if (venda == null)
                return CreateErrorResult<bool>("Venda não encontrada");

            if (venda.Status == VendaStatus.Finalizada)
                return CreateErrorResult<bool>("Venda já finalizada não pode ser cancelada");

            if (venda.Status == VendaStatus.Cancelada)
                return CreateErrorResult<bool>("Venda já está cancelada");

            // Liberar estoque reservado
            foreach (var item in venda.Itens)
            {
                var produto = await _produtoRepository.GetByIdAsync(item.ProdutoId, cancellationToken);
                if (produto != null)
                {
                    produto.LiberarReserva(item.Quantidade);
                    await _produtoRepository.UpdateAsync(produto, cancellationToken);
                }
            }

            // Cancelar venda
            venda.Cancelar(request.Motivo);
            await _vendaRepository.UpdateAsync(venda, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Venda cancelada com sucesso: {VendaId}", venda.Id);
            return new BusinessResult<bool>(true);
        }
        catch (Domain.Exceptions.DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao cancelar venda");
            return CreateErrorResult<bool>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao cancelar venda");
            return CreateErrorResult<bool>("Erro interno ao cancelar venda");
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
