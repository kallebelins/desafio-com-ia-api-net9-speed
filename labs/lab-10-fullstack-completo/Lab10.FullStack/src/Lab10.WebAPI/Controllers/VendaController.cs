using Microsoft.AspNetCore.Mvc;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;
using Lab10.Application.Commands.Vendas;
using Lab10.Application.DTOs;
using Lab10.Application.Queries.Vendas;

namespace Lab10.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendaController : ControllerBase
{
    private readonly IMediator _mediator;

    public VendaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Busca venda por ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.SendAsync(new GetVendaByIdQuery(id));
        
        if (result.Data == null && result.HasErrors)
            return NotFound(result);
            
        return Ok(result);
    }

    /// <summary>
    /// Lista vendas de um cliente
    /// </summary>
    [HttpGet("cliente/{clienteId:int}")]
    public async Task<IActionResult> GetByCliente(int clienteId)
    {
        var result = await _mediator.SendAsync(new GetVendasByClienteQuery(clienteId));
        return Ok(result);
    }

    /// <summary>
    /// Inicia uma nova venda (executa Saga)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> IniciarVenda([FromBody] IniciarVendaDto dto)
    {
        var command = new IniciarVendaCommand(dto.ClienteId, dto.Itens);
        var result = await _mediator.SendAsync(command);

        if (result.HasErrors)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Finaliza uma venda (processa pagamento)
    /// </summary>
    [HttpPost("{id:int}/finalizar")]
    public async Task<IActionResult> FinalizarVenda(int id, [FromBody] ProcessarPagamentoDto dto)
    {
        var command = new FinalizarVendaCommand(id, dto.Metodo);
        var result = await _mediator.SendAsync(command);

        if (result.HasErrors)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Cancela uma venda
    /// </summary>
    [HttpPost("{id:int}/cancelar")]
    public async Task<IActionResult> CancelarVenda(int id, [FromBody] string? motivo = null)
    {
        var command = new CancelarVendaCommand(id, motivo);
        var result = await _mediator.SendAsync(command);

        if (result.HasErrors)
            return BadRequest(result);

        return Ok(result);
    }
}
