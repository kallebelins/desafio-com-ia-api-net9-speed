using Microsoft.AspNetCore.Mvc;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;
using Lab10.Application.Commands.Clientes;
using Lab10.Application.DTOs;
using Lab10.Application.Queries.Clientes;

namespace Lab10.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClienteController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClienteController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lista todos os clientes
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool apenasAtivos = false)
    {
        var result = await _mediator.SendAsync(new GetAllClientesQuery(apenasAtivos));
        return Ok(result);
    }

    /// <summary>
    /// Busca cliente por ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.SendAsync(new GetClienteByIdQuery(id));
        
        if (result.Data == null && result.HasErrors)
            return NotFound(result);
            
        return Ok(result);
    }

    /// <summary>
    /// Cria um novo cliente
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ClienteCreateDto dto)
    {
        var command = new CreateClienteCommand(dto.Nome, dto.Email, dto.Cpf, dto.Endereco);
        var result = await _mediator.SendAsync(command);

        if (result.HasErrors)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Atualiza um cliente existente
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ClienteUpdateDto dto)
    {
        var command = new UpdateClienteCommand(id, dto.Nome, dto.Email, dto.Endereco);
        var result = await _mediator.SendAsync(command);

        if (result.HasErrors)
            return BadRequest(result);

        if (result.Data == null)
            return NotFound(result);

        return Ok(result);
    }
}
