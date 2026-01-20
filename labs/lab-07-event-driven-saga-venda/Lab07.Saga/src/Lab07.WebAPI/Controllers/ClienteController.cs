using Lab07.Application.Services;
using Lab07.Core.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace Lab07.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClienteController : ControllerBase
{
    private readonly IClienteService _clienteService;
    private readonly ILogger<ClienteController> _logger;

    public ClienteController(
        IClienteService clienteService,
        ILogger<ClienteController> logger)
    {
        _clienteService = clienteService;
        _logger = logger;
    }

    /// <summary>
    /// Lista todos os clientes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClienteDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ClienteDto>>> GetAll(CancellationToken cancellationToken)
    {
        var clientes = await _clienteService.GetAllAsync(cancellationToken);
        return Ok(clientes);
    }

    /// <summary>
    /// Obtém um cliente pelo ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var cliente = await _clienteService.GetByIdAsync(id, cancellationToken);
        
        if (cliente == null)
            return NotFound();

        return Ok(cliente);
    }

    /// <summary>
    /// Cria um novo cliente
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClienteDto>> Create([FromBody] CreateClienteRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            return BadRequest("Nome é obrigatório");

        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest("Email é obrigatório");

        var cliente = await _clienteService.CreateAsync(
            request.Nome,
            request.Email,
            request.Telefone ?? string.Empty,
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
    }

    /// <summary>
    /// Atualiza um cliente
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteDto>> Update(Guid id, [FromBody] UpdateClienteRequest request, CancellationToken cancellationToken)
    {
        var cliente = await _clienteService.UpdateAsync(
            id,
            request.Nome,
            request.Email,
            request.Telefone ?? string.Empty,
            cancellationToken);

        if (cliente == null)
            return NotFound();

        return Ok(cliente);
    }

    /// <summary>
    /// Remove um cliente
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var success = await _clienteService.DeleteAsync(id, cancellationToken);
        
        if (!success)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Ativa um cliente
    /// </summary>
    [HttpPatch("{id:guid}/ativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Ativar(Guid id, CancellationToken cancellationToken)
    {
        var success = await _clienteService.AtivarAsync(id, cancellationToken);
        
        if (!success)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Desativa um cliente
    /// </summary>
    [HttpPatch("{id:guid}/desativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Desativar(Guid id, CancellationToken cancellationToken)
    {
        var success = await _clienteService.DesativarAsync(id, cancellationToken);
        
        if (!success)
            return NotFound();

        return NoContent();
    }
}

public record CreateClienteRequest(string Nome, string Email, string? Telefone);
public record UpdateClienteRequest(string Nome, string Email, string? Telefone);
