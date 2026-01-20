using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Core.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;
using Microsoft.Extensions.Logging;
using Lab10.Application.Commands.Vendas;
using Lab10.Application.DTOs;
using Lab10.Application.Interfaces;
using Lab10.Application.Sagas;
using Lab10.Domain.Interfaces;

namespace Lab10.Application.Handlers.Commands;

public class IniciarVendaCommandHandler : IMediatorCommandHandler<IniciarVendaCommand, IBusinessResult<VendaDto>>
{
    private readonly ProcessarVendaSaga _processarVendaSaga;
    private readonly IClienteRepository _clienteRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IUnitOfWorkApplication _unitOfWork;
    private readonly ILogger<IniciarVendaCommandHandler> _logger;

    public IniciarVendaCommandHandler(
        ProcessarVendaSaga processarVendaSaga,
        IClienteRepository clienteRepository,
        IProdutoRepository produtoRepository,
        IUnitOfWorkApplication unitOfWork,
        ILogger<IniciarVendaCommandHandler> logger)
    {
        _processarVendaSaga = processarVendaSaga;
        _clienteRepository = clienteRepository;
        _produtoRepository = produtoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IBusinessResult<VendaDto>> Handle(IniciarVendaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Iniciando venda para cliente: {ClienteId}", request.ClienteId);

            // Buscar cliente
            var cliente = await _clienteRepository.GetByIdAsync(request.ClienteId, cancellationToken);
            if (cliente == null)
                return CreateErrorResult<VendaDto>("Cliente não encontrado");

            // Buscar produtos
            var produtos = new List<Domain.Entities.Produto>();
            foreach (var item in request.Itens)
            {
                var produto = await _produtoRepository.GetByIdAsync(item.ProdutoId, cancellationToken);
                if (produto == null)
                    return CreateErrorResult<VendaDto>($"Produto com Id {item.ProdutoId} não encontrado");
                produtos.Add(produto);
            }

            // Criar contexto da Saga
            var context = new ProcessarVendaSagaContext
            {
                ClienteId = request.ClienteId,
                Cliente = cliente,
                ItensSolicitados = request.Itens.Select(i => new ItemSolicitado
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade
                }).ToList(),
                Produtos = produtos
            };

            // Executar Saga
            var sagaResult = await _processarVendaSaga.ExecuteAsync(context, cancellationToken);

            if (!sagaResult.Sucesso)
            {
                _logger.LogWarning("Saga de venda falhou: {Error}", sagaResult.Erro);
                return CreateErrorResult<VendaDto>(sagaResult.Erro ?? "Erro ao processar venda");
            }

            _logger.LogInformation("Saga de venda concluída com sucesso. VendaId: {VendaId}", context.Venda?.Id);

            // Montar DTO de resposta
            var venda = context.Venda!;
            var dto = new VendaDto(
                venda.Id,
                venda.ClienteId,
                cliente.Nome,
                venda.Status,
                venda.ValorTotal.Valor,
                venda.DataCriacao,
                venda.DataFinalizacao,
                venda.Observacao,
                venda.Itens.Select(i => new ItemVendaDto(
                    i.Id,
                    i.ProdutoId,
                    i.ProdutoNome,
                    i.Quantidade,
                    i.PrecoUnitario.Valor,
                    i.ValorTotal.Valor)),
                null);

            return new BusinessResult<VendaDto>(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar Saga de venda");
            return CreateErrorResult<VendaDto>("Erro interno ao processar venda");
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
