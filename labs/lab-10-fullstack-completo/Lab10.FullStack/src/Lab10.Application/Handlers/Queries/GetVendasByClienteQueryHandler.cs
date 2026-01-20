using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;
using Lab10.Application.DTOs;
using Lab10.Application.Queries.Vendas;
using Lab10.Domain.Interfaces;

namespace Lab10.Application.Handlers.Queries;

public class GetVendasByClienteQueryHandler : IMediatorQueryHandler<GetVendasByClienteQuery, IBusinessResult<IEnumerable<VendaResumoDto>>>
{
    private readonly IVendaRepository _vendaRepository;

    public GetVendasByClienteQueryHandler(IVendaRepository vendaRepository)
    {
        _vendaRepository = vendaRepository;
    }

    public async Task<IBusinessResult<IEnumerable<VendaResumoDto>>> Handle(GetVendasByClienteQuery request, CancellationToken cancellationToken)
    {
        var vendas = await _vendaRepository.GetByClienteAsync(request.ClienteId, cancellationToken);

        var dtos = vendas.Select(v => new VendaResumoDto(
            v.Id,
            v.ClienteId,
            v.Cliente?.Nome,
            v.Status,
            v.ValorTotal.Valor,
            v.Itens.Count,
            v.DataCriacao));

        return new BusinessResult<IEnumerable<VendaResumoDto>>(dtos);
    }
}
