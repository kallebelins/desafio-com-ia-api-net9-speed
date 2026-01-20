using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Core.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;
using Lab10.Application.DTOs;
using Lab10.Application.Queries.Vendas;
using Lab10.Domain.Enums;
using Lab10.Domain.Interfaces;

namespace Lab10.Application.Handlers.Queries;

public class GetRelatorioVendasQueryHandler : IMediatorQueryHandler<GetRelatorioVendasQuery, IBusinessResult<RelatorioVendasDto>>
{
    private readonly IVendaRepository _vendaRepository;

    public GetRelatorioVendasQueryHandler(IVendaRepository vendaRepository)
    {
        _vendaRepository = vendaRepository;
    }

    public async Task<IBusinessResult<RelatorioVendasDto>> Handle(GetRelatorioVendasQuery request, CancellationToken cancellationToken)
    {
        var vendas = await _vendaRepository.GetByPeriodoAsync(request.Inicio, request.Fim, cancellationToken);

        var totalVendas = vendas.Count();
        var valorTotal = vendas.Where(v => v.Status == VendaStatus.Finalizada).Sum(v => v.ValorTotal.Valor);
        var finalizadas = vendas.Count(v => v.Status == VendaStatus.Finalizada);
        var canceladas = vendas.Count(v => v.Status == VendaStatus.Cancelada);
        var pendentes = vendas.Count(v => v.Status == VendaStatus.Pendente || v.Status == VendaStatus.EmProcessamento);
        
        var ticketMedio = finalizadas > 0 ? valorTotal / finalizadas : 0;

        var dto = new RelatorioVendasDto(
            request.Inicio,
            request.Fim,
            totalVendas,
            valorTotal,
            ticketMedio,
            finalizadas,
            canceladas,
            pendentes);

        return new BusinessResult<RelatorioVendasDto>(dto);
    }
}
