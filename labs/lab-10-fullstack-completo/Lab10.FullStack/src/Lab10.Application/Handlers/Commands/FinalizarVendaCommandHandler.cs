using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Core.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;
using Microsoft.Extensions.Logging;
using Lab10.Application.Commands.Vendas;
using Lab10.Application.DTOs;
using Lab10.Application.Interfaces;
using Lab10.Domain.Entities;
using Lab10.Domain.Enums;
using Lab10.Domain.Events.Integration;
using Lab10.Domain.Interfaces;
using Lab10.Domain.ValueObjects;

namespace Lab10.Application.Handlers.Commands;

public class FinalizarVendaCommandHandler : IMediatorCommandHandler<FinalizarVendaCommand, IBusinessResult<VendaDto>>
{
    private readonly IVendaRepository _vendaRepository;
    private readonly IPagamentoRepository _pagamentoRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IPagamentoGateway _pagamentoGateway;
    private readonly IOutboxService _outboxService;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWorkApplication _unitOfWork;
    private readonly ILogger<FinalizarVendaCommandHandler> _logger;

    public FinalizarVendaCommandHandler(
        IVendaRepository vendaRepository,
        IPagamentoRepository pagamentoRepository,
        IProdutoRepository produtoRepository,
        IPagamentoGateway pagamentoGateway,
        IOutboxService outboxService,
        IEmailService emailService,
        IUnitOfWorkApplication unitOfWork,
        ILogger<FinalizarVendaCommandHandler> logger)
    {
        _vendaRepository = vendaRepository;
        _pagamentoRepository = pagamentoRepository;
        _produtoRepository = produtoRepository;
        _pagamentoGateway = pagamentoGateway;
        _outboxService = outboxService;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IBusinessResult<VendaDto>> Handle(FinalizarVendaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Finalizando venda: {VendaId}", request.VendaId);

            var venda = await _vendaRepository.GetByIdWithItensAsync(request.VendaId, cancellationToken);
            if (venda == null)
                return CreateErrorResult<VendaDto>("Venda não encontrada");

            if (venda.Status != VendaStatus.Pendente && venda.Status != VendaStatus.Confirmada && venda.Status != VendaStatus.EmProcessamento)
                return CreateErrorResult<VendaDto>($"Venda não pode ser finalizada. Status atual: {venda.Status}");

            // Marcar como em processamento
            venda.IniciarProcessamento();

            // Criar pagamento
            var valorPagamento = Money.Create(venda.ValorTotal.Valor, "BRL");
            var pagamento = new Pagamento(venda.Id, valorPagamento, request.MetodoPagamento);

            // Processar pagamento
            var resultadoPagamento = await _pagamentoGateway.ProcessarPagamentoAsync(
                venda.ClienteId,
                pagamento.Valor.Valor,
                request.MetodoPagamento,
                cancellationToken);

            if (resultadoPagamento.Sucesso)
            {
                pagamento.ProcessarComSucesso(resultadoPagamento.TransacaoId ?? Guid.NewGuid().ToString());
                venda.Finalizar();

                // Confirmar estoque reservado (baixar estoque)
                foreach (var item in venda.Itens)
                {
                    var produto = await _produtoRepository.GetByIdAsync(item.ProdutoId, cancellationToken);
                    if (produto != null)
                    {
                        produto.BaixarEstoque(item.Quantidade);
                        await _produtoRepository.UpdateAsync(produto, cancellationToken);
                    }
                }

                // Enviar email de confirmação
                if (venda.Cliente != null)
                {
                    await _emailService.EnviarEmailConfirmacaoVendaAsync(
                        venda.Cliente.Email.Valor,
                        venda.Cliente.Nome,
                        venda.Id,
                        venda.ValorTotal.Valor,
                        cancellationToken);
                }

                // Publicar evento de integração
                var integrationEvent = new VendaFinalizadaIntegrationEvent(
                    venda.Id,
                    venda.ClienteId,
                    venda.Cliente?.Email.Valor ?? string.Empty,
                    venda.ValorTotal.Valor,
                    venda.Itens.Count,
                    DateTime.UtcNow);
                await _outboxService.AddMessageAsync(integrationEvent, cancellationToken);
            }
            else
            {
                pagamento.Rejeitar(resultadoPagamento.MotivoFalha ?? "Pagamento rejeitado");
                // Manter venda em processamento para retry
            }

            await _pagamentoRepository.AddAsync(pagamento, cancellationToken);
            await _vendaRepository.UpdateAsync(venda, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Montar DTO
            var dto = new VendaDto(
                venda.Id,
                venda.ClienteId,
                venda.Cliente?.Nome,
                venda.Status,
                venda.ValorTotal.Valor,
                venda.DataCriacao,
                venda.DataFinalizacao,
                venda.Observacao,
                venda.Itens.Select(i => new ItemVendaDto(
                    i.Id, i.ProdutoId, i.ProdutoNome, i.Quantidade,
                    i.PrecoUnitario.Valor, i.ValorTotal.Valor)),
                new PagamentoDto(
                    pagamento.Id, pagamento.VendaId, pagamento.Valor.Valor,
                    pagamento.Metodo, pagamento.Status, pagamento.TransacaoId,
                    pagamento.DataCriacao, pagamento.DataProcessamento, pagamento.MotivoFalha));

            if (!resultadoPagamento.Sucesso)
            {
                _logger.LogWarning("Pagamento rejeitado para venda {VendaId}: {Motivo}", venda.Id, resultadoPagamento.MotivoFalha);
                return CreateErrorResultWithData(dto, $"Pagamento rejeitado: {resultadoPagamento.MotivoFalha}");
            }

            _logger.LogInformation("Venda finalizada com sucesso: {VendaId}", venda.Id);
            return new BusinessResult<VendaDto>(dto);
        }
        catch (Domain.Exceptions.DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao finalizar venda");
            return CreateErrorResult<VendaDto>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao finalizar venda");
            return CreateErrorResult<VendaDto>("Erro interno ao finalizar venda");
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

    private static IBusinessResult<VendaDto> CreateErrorResultWithData(VendaDto data, string message)
    {
        IReadOnlyCollection<IMessageResult> messages = new List<IMessageResult>
        {
            new MessageResult(message, MessageType.Warning)
        };
        return new BusinessResult<VendaDto>(data, messages);
    }
}
