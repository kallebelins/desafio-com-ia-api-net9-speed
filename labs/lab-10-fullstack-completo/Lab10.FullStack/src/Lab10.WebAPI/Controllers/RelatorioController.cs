using Microsoft.AspNetCore.Mvc;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;
using Lab10.Application.Queries.Vendas;

namespace Lab10.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RelatorioController : ControllerBase
{
    private readonly IMediator _mediator;

    public RelatorioController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtém relatório de vendas por período
    /// </summary>
    [HttpGet("vendas")]
    public async Task<IActionResult> GetRelatorioVendas(
        [FromQuery] DateTime? inicio = null, 
        [FromQuery] DateTime? fim = null)
    {
        var dataInicio = inicio ?? DateTime.UtcNow.AddDays(-30);
        var dataFim = fim ?? DateTime.UtcNow;

        var result = await _mediator.SendAsync(new GetRelatorioVendasQuery(dataInicio, dataFim));
        return Ok(result);
    }
}
