using Lab08.Application.Interfaces;
using Lab08.Application.UseCases.Categorias.CreateCategoria;
using Lab08.Application.UseCases.Categorias.ListCategorias;
using Microsoft.AspNetCore.Mvc;

namespace Lab08.WebAPI.Controllers;

/// <summary>
/// Controller para operações de Categoria
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CategoriaController : ControllerBase
{
    private readonly IUseCase<CreateCategoriaInput, CreateCategoriaOutput> _createCategoria;
    private readonly IUseCase<ListCategoriasInput, ListCategoriasOutput> _listCategorias;

    public CategoriaController(
        IUseCase<CreateCategoriaInput, CreateCategoriaOutput> createCategoria,
        IUseCase<ListCategoriasInput, ListCategoriasOutput> listCategorias)
    {
        _createCategoria = createCategoria;
        _listCategorias = listCategorias;
    }

    /// <summary>
    /// Lista todas as categorias
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool apenasAtivas = false, CancellationToken cancellationToken = default)
    {
        var input = new ListCategoriasInput { ApenasAtivas = apenasAtivas };
        var result = await _listCategorias.ExecuteAsync(input, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Cria uma nova categoria
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoriaInput input, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input.Nome))
            return BadRequest(new { ErrorMessage = "Nome é obrigatório" });

        var result = await _createCategoria.ExecuteAsync(input, cancellationToken);

        if (!result.Success)
            return BadRequest(new { result.ErrorMessage });

        return Created($"/api/categoria/{result.Categoria!.Id}", result.Categoria);
    }
}
