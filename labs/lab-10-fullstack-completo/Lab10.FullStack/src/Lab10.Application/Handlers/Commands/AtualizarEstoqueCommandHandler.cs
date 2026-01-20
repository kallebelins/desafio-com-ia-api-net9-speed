using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Core.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;
using Microsoft.Extensions.Logging;
using Lab10.Application.Commands.Produtos;
using Lab10.Application.DTOs;
using Lab10.Application.Interfaces;
using Lab10.Domain.Interfaces;

namespace Lab10.Application.Handlers.Commands;

public class AtualizarEstoqueCommandHandler : IMediatorCommandHandler<AtualizarEstoqueCommand, IBusinessResult<ProdutoDto>>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IUnitOfWorkApplication _unitOfWork;
    private readonly ILogger<AtualizarEstoqueCommandHandler> _logger;

    public AtualizarEstoqueCommandHandler(
        IProdutoRepository produtoRepository,
        IUnitOfWorkApplication unitOfWork,
        ILogger<AtualizarEstoqueCommandHandler> logger)
    {
        _produtoRepository = produtoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IBusinessResult<ProdutoDto>> Handle(AtualizarEstoqueCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Atualizando estoque do produto: {ProdutoId}, Quantidade: {Quantidade}", 
                request.ProdutoId, request.Quantidade);

            var produto = await _produtoRepository.GetByIdAsync(request.ProdutoId, cancellationToken);
            if (produto == null)
                return CreateErrorResult<ProdutoDto>("Produto não encontrado");

            // Adicionar estoque (pode ser positivo ou negativo)
            if (request.Quantidade > 0)
            {
                produto.AdicionarEstoque(request.Quantidade);
            }
            else if (request.Quantidade < 0)
            {
                // Para reduzir estoque, reservamos e baixamos
                var quantidade = Math.Abs(request.Quantidade);
                produto.ReservarEstoque(quantidade);
                produto.BaixarEstoque(quantidade);
            }

            await _produtoRepository.UpdateAsync(produto, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = new ProdutoDto(
                produto.Id,
                produto.Nome,
                produto.Descricao,
                produto.PrecoUnitario.Valor,
                produto.EstoqueAtual,
                produto.EstoqueReservado,
                produto.EstoqueDisponivel,
                produto.CategoriaId,
                produto.Categoria?.Nome,
                produto.Ativo);

            _logger.LogInformation("Estoque atualizado com sucesso. Novo estoque: {Estoque}", produto.EstoqueAtual);
            return new BusinessResult<ProdutoDto>(dto);
        }
        catch (Domain.Exceptions.DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao atualizar estoque");
            return CreateErrorResult<ProdutoDto>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar estoque");
            return CreateErrorResult<ProdutoDto>("Erro interno ao atualizar estoque");
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
