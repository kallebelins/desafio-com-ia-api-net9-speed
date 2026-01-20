using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Core.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;
using Microsoft.Extensions.Logging;
using Lab10.Application.Commands.Produtos;
using Lab10.Application.DTOs;
using Lab10.Application.Interfaces;
using Lab10.Domain.Entities;
using Lab10.Domain.Interfaces;
using Lab10.Domain.ValueObjects;

namespace Lab10.Application.Handlers.Commands;

public class CreateProdutoCommandHandler : IMediatorCommandHandler<CreateProdutoCommand, IBusinessResult<ProdutoDto>>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IUnitOfWorkApplication _unitOfWork;
    private readonly ILogger<CreateProdutoCommandHandler> _logger;

    public CreateProdutoCommandHandler(
        IProdutoRepository produtoRepository,
        ICategoriaRepository categoriaRepository,
        IUnitOfWorkApplication unitOfWork,
        ILogger<CreateProdutoCommandHandler> logger)
    {
        _produtoRepository = produtoRepository;
        _categoriaRepository = categoriaRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IBusinessResult<ProdutoDto>> Handle(CreateProdutoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Criando produto: {Nome}", request.Nome);

            // Verificar se categoria existe
            var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId, cancellationToken);
            if (categoria == null)
                return CreateErrorResult<ProdutoDto>("Categoria não encontrada");

            // Criar Value Object Money
            var preco = Money.Create(request.PrecoUnitario, "BRL");

            // Criar entidade
            var produto = new Produto(
                request.Nome,
                request.Descricao,
                preco,
                request.EstoqueInicial,
                request.CategoriaId);
            
            await _produtoRepository.AddAsync(produto, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Criar DTO de resposta
            var dto = new ProdutoDto(
                produto.Id,
                produto.Nome,
                produto.Descricao,
                produto.PrecoUnitario.Valor,
                produto.EstoqueAtual,
                produto.EstoqueReservado,
                produto.EstoqueDisponivel,
                produto.CategoriaId,
                categoria.Nome,
                produto.Ativo);

            _logger.LogInformation("Produto criado com sucesso: {Id}", produto.Id);
            return new BusinessResult<ProdutoDto>(dto);
        }
        catch (Domain.Exceptions.DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao criar produto");
            return CreateErrorResult<ProdutoDto>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar produto");
            return CreateErrorResult<ProdutoDto>("Erro interno ao criar produto");
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
