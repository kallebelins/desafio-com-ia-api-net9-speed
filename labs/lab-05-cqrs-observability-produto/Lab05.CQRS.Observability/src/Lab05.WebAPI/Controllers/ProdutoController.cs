using Lab05.Application.Commands;
using Lab05.Application.Queries;
using Lab05.Core.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab05.WebAPI.Controllers;

/// <summary>
/// Controller para operações de Produto
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProdutoController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProdutoController> _logger;

    public ProdutoController(IMediator mediator, ILogger<ProdutoController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Lista todos os produtos
    /// </summary>
    /// <param name="apenasAtivos">Filtrar apenas produtos ativos</param>
    /// <param name="categoria">Filtrar por categoria</param>
    [HttpGet]
    [ProducesResponseType(typeof(IBusinessResult<IList<ProdutoDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool? apenasAtivos = null,
        [FromQuery] string? categoria = null)
    {
        _logger.LogInformation("GET /api/produto - ApenasAtivos: {ApenasAtivos}, Categoria: {Categoria}",
            apenasAtivos, categoria);

        var query = new GetProdutosQuery(apenasAtivos, categoria);
        var result = await _mediator.SendAsync(query);

        return Ok(result);
    }

    /// <summary>
    /// Busca um produto por ID
    /// </summary>
    /// <param name="id">ID do produto</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(IBusinessResult<ProdutoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        _logger.LogInformation("GET /api/produto/{Id}", id);

        var query = new GetProdutoByIdQuery(id);
        var result = await _mediator.SendAsync(query);

        if (result.Data == null && result.HasErrors)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Busca um produto por SKU
    /// </summary>
    /// <param name="sku">SKU do produto</param>
    [HttpGet("sku/{sku}")]
    [ProducesResponseType(typeof(IBusinessResult<ProdutoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBySKU(string sku)
    {
        _logger.LogInformation("GET /api/produto/sku/{SKU}", sku);

        var query = new GetProdutoBySKUQuery(sku);
        var result = await _mediator.SendAsync(query);

        if (result.Data == null && result.HasErrors)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Cria um novo produto
    /// </summary>
    /// <param name="dto">Dados do produto</param>
    [HttpPost]
    [ProducesResponseType(typeof(IBusinessResult<ProdutoDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProdutoDto dto)
    {
        _logger.LogInformation("POST /api/produto - SKU: {SKU}", dto.SKU);

        var command = new CreateProdutoCommand(dto);
        var result = await _mediator.SendAsync(command);

        if (result.HasErrors)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Atualiza um produto existente
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <param name="dto">Dados atualizados do produto</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(IBusinessResult<ProdutoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProdutoDto dto)
    {
        _logger.LogInformation("PUT /api/produto/{Id}", id);

        if (id != dto.Id)
        {
            return BadRequest("ID da URL não corresponde ao ID do corpo da requisição");
        }

        var command = new UpdateProdutoCommand(dto);
        var result = await _mediator.SendAsync(command);

        if (result.HasErrors)
        {
            if (result.Messages.Any(m => m.Message.Contains("não encontrado")))
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Remove um produto
    /// </summary>
    /// <param name="id">ID do produto</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(IBusinessResult<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        _logger.LogInformation("DELETE /api/produto/{Id}", id);

        var command = new DeleteProdutoCommand(id);
        var result = await _mediator.SendAsync(command);

        if (result.HasErrors)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}
