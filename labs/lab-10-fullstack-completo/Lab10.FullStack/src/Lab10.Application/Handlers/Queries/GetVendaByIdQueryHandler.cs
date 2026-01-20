using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Core.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;
using Lab10.Application.DTOs;
using Lab10.Application.Queries.Vendas;
using Lab10.Domain.Interfaces;

namespace Lab10.Application.Handlers.Queries;

public class GetVendaByIdQueryHandler : IMediatorQueryHandler<GetVendaByIdQuery, IBusinessResult<VendaDto>>
{
    private readonly IVendaRepository _vendaRepository;

    public GetVendaByIdQueryHandler(IVendaRepository vendaRepository)
    {
        _vendaRepository = vendaRepository;
    }

    public async Task<IBusinessResult<VendaDto>> Handle(GetVendaByIdQuery request, CancellationToken cancellationToken)
    {
        var venda = await _vendaRepository.GetByIdWithItensAsync(request.Id, cancellationToken);
        
        if (venda == null)
            return CreateErrorResult<VendaDto>("Venda não encontrada");

        PagamentoDto? pagamentoDto = null;
        if (venda.Pagamento != null)
        {
            pagamentoDto = new PagamentoDto(
                venda.Pagamento.Id,
                venda.Pagamento.VendaId,
                venda.Pagamento.Valor.Valor,
                venda.Pagamento.Metodo,
                venda.Pagamento.Status,
                venda.Pagamento.TransacaoId,
                venda.Pagamento.DataCriacao,
                venda.Pagamento.DataProcessamento,
                venda.Pagamento.MotivoFalha);
        }

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
                i.Id,
                i.ProdutoId,
                i.ProdutoNome,
                i.Quantidade,
                i.PrecoUnitario.Valor,
                i.ValorTotal.Valor)),
            pagamentoDto);

        return new BusinessResult<VendaDto>(dto);
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
