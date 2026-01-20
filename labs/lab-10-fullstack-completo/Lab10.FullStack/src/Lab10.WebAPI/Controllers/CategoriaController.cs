using Microsoft.AspNetCore.Mvc;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;
using Lab10.Application.Commands.Categorias;
using Lab10.Application.DTOs;
using Lab10.Application.Queries.Categorias;

namespace Lab10.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriaController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lista todas as categorias
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool apenasAtivas = false)
    {
        var result = await _mediator.SendAsync(new GetAllCategoriasQuery(apenasAtivas));
        return Ok(result);
    }

    /// <summary>
    /// Cria uma nova categoria
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoriaCreateDto dto)
    {
        var command = new CreateCategoriaCommand(dto.Nome, dto.Descricao);
        var result = await _mediator.SendAsync(command);

        if (result.HasErrors)
            return BadRequest(result);

        return Created("", result);
    }
}
