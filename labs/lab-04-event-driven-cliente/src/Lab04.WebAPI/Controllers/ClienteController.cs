using Lab04.Core.Contract.Services;
using Lab04.Core.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace Lab04.WebAPI.Controllers;

/// <summary>
/// Controller de Clientes
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
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
    public async Task<ActionResult<IEnumerable<ClienteDto>>> GetAll(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GET /api/cliente");
        
        var result = await _clienteService.GetAllAsync(cancellationToken);
        
        if (result.HasErrors)
            return BadRequest(result.Messages);
        
        return Ok(result.Data);
    }

    /// <summary>
    /// Obtém um cliente por ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteDto>> GetById(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GET /api/cliente/{Id}", id);
        
        var result = await _clienteService.GetByIdAsync(id, cancellationToken);
        
        if (result.HasErrors)
            return NotFound(result.Messages);
        
        if (result.Data == null)
            return NotFound();
        
        return Ok(result.Data);
    }

    /// <summary>
    /// Cria um novo cliente
    /// Dispara evento ClienteCriado que envia email de boas-vindas via RabbitMQ
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClienteDto>> Create(
        [FromBody] ClienteCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("POST /api/cliente - Nome: {Nome}", dto.Nome);
        
        var result = await _clienteService.CreateAsync(dto, cancellationToken);
        
        if (result.HasErrors)
            return BadRequest(result.Messages);
        
        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data!.Id },
            result.Data);
    }

    /// <summary>
    /// Atualiza um cliente existente
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClienteDto>> Update(
        int id,
        [FromBody] ClienteUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("PUT /api/cliente/{Id}", id);
        
        var result = await _clienteService.UpdateAsync(id, dto, cancellationToken);
        
        if (result.HasErrors)
        {
            if (result.Messages.Any(m => m.Message?.Contains("não encontrado") == true))
                return NotFound(result.Messages);
            return BadRequest(result.Messages);
        }
        
        return Ok(result.Data);
    }

    /// <summary>
    /// Exclui um cliente
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("DELETE /api/cliente/{Id}", id);
        
        var result = await _clienteService.DeleteAsync(id, cancellationToken);
        
        if (result.HasErrors)
            return NotFound(result.Messages);
        
        return NoContent();
    }
}
