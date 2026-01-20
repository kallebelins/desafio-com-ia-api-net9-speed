using Microsoft.AspNetCore.Mvc;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;
using Lab10.Application.Commands.Produtos;
using Lab10.Application.DTOs;
using Lab10.Application.Queries.Produtos;

namespace Lab10.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutoController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProdutoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lista todos os produtos
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool apenasAtivos = false, 
        [FromQuery] bool apenasComEstoque = false)
    {
        var result = await _mediator.SendAsync(new GetAllProdutosQuery(apenasAtivos, apenasComEstoque));
        return Ok(result);
    }

    /// <summary>
    /// Busca produto por ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.SendAsync(new GetProdutoByIdQuery(id));
        
        if (result.Data == null && result.HasErrors)
            return NotFound(result);
            
        return Ok(result);
    }

    /// <summary>
    /// Lista produtos por categoria
    /// </summary>
    [HttpGet("categoria/{categoriaId:int}")]
    public async Task<IActionResult> GetByCategoria(int categoriaId)
    {
        var result = await _mediator.SendAsync(new GetProdutosByCategoriaQuery(categoriaId));
        return Ok(result);
    }

    /// <summary>
    /// Cria um novo produto
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProdutoCreateDto dto)
    {
        var command = new CreateProdutoCommand(
            dto.Nome, dto.Descricao, dto.PrecoUnitario, dto.EstoqueInicial, dto.CategoriaId);
        var result = await _mediator.SendAsync(command);

        if (result.HasErrors)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Adiciona estoque a um produto
    /// </summary>
    [HttpPost("{id:int}/estoque")]
    public async Task<IActionResult> AtualizarEstoque(int id, [FromBody] AtualizarEstoqueDto dto)
    {
        var command = new AtualizarEstoqueCommand(id, dto.Quantidade);
        var result = await _mediator.SendAsync(command);

        if (result.HasErrors)
            return BadRequest(result);

        if (result.Data == null)
            return NotFound(result);

        return Ok(result);
    }
}
