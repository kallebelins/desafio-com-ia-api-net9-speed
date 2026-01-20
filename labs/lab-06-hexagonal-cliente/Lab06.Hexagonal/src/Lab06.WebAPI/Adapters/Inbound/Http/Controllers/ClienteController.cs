using Lab06.Application.DTOs.Requests;
using Lab06.Application.Ports.Inbound;
using Microsoft.AspNetCore.Mvc;

namespace Lab06.WebAPI.Adapters.Inbound.Http.Controllers;

/// <summary>
/// Inbound Adapter (HTTP) - Controller REST para gerenciamento de clientes
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ClienteController : ControllerBase
{
    private readonly ICreateClienteUseCase _createClienteUseCase;
    private readonly IGetClienteUseCase _getClienteUseCase;
    private readonly IUpdateClienteUseCase _updateClienteUseCase;
    private readonly IDeleteClienteUseCase _deleteClienteUseCase;

    public ClienteController(
        ICreateClienteUseCase createClienteUseCase,
        IGetClienteUseCase getClienteUseCase,
        IUpdateClienteUseCase updateClienteUseCase,
        IDeleteClienteUseCase deleteClienteUseCase)
    {
        _createClienteUseCase = createClienteUseCase;
        _getClienteUseCase = getClienteUseCase;
        _updateClienteUseCase = updateClienteUseCase;
        _deleteClienteUseCase = deleteClienteUseCase;
    }

    /// <summary>
    /// Lista todos os clientes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _getClienteUseCase.ExecuteAllAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Lista todos os clientes ativos
    /// </summary>
    [HttpGet("ativos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAtivos(CancellationToken cancellationToken)
    {
        var result = await _getClienteUseCase.ExecuteAllAtivosAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Busca um cliente por ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _getClienteUseCase.ExecuteByIdAsync(id, cancellationToken);
        
        if (result.Data == null || result.HasErrors)
            return NotFound(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Cria um novo cliente
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateClienteRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createClienteUseCase.ExecuteAsync(request, cancellationToken);
        
        if (result.HasErrors)
            return BadRequest(result);
        
        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data?.Id },
            result);
    }

    /// <summary>
    /// Atualiza um cliente existente
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateClienteRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _updateClienteUseCase.ExecuteAsync(id, request, cancellationToken);
        
        if (result.HasErrors)
        {
            if (result.Messages?.Any(m => m.Message?.Contains("não encontrado") == true) == true)
                return NotFound(result);
            
            return BadRequest(result);
        }
        
        return Ok(result);
    }

    /// <summary>
    /// Remove um cliente
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _deleteClienteUseCase.ExecuteAsync(id, cancellationToken);
        
        if (result.HasErrors)
            return NotFound(result);
        
        return NoContent();
    }
}
