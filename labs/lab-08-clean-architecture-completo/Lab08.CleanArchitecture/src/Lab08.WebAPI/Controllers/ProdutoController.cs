using FluentValidation;
using Lab08.Application.Interfaces;
using Lab08.Application.UseCases.Produtos.CreateProduto;
using Lab08.Application.UseCases.Produtos.GetProduto;
using Lab08.Application.UseCases.Produtos.ListProdutos;
using Microsoft.AspNetCore.Mvc;

namespace Lab08.WebAPI.Controllers;

/// <summary>
/// Controller para operações de Produto
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProdutoController : ControllerBase
{
    private readonly IUseCase<CreateProdutoInput, CreateProdutoOutput> _createProduto;
    private readonly IUseCase<GetProdutoInput, GetProdutoOutput> _getProduto;
    private readonly IUseCase<ListProdutosInput, ListProdutosOutput> _listProdutos;
    private readonly IValidator<CreateProdutoInput> _createValidator;

    public ProdutoController(
        IUseCase<CreateProdutoInput, CreateProdutoOutput> createProduto,
        IUseCase<GetProdutoInput, GetProdutoOutput> getProduto,
        IUseCase<ListProdutosInput, ListProdutosOutput> listProdutos,
        IValidator<CreateProdutoInput> createValidator)
    {
        _createProduto = createProduto;
        _getProduto = getProduto;
        _listProdutos = listProdutos;
        _createValidator = createValidator;
    }

    /// <summary>
    /// Lista todos os produtos
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool apenasAtivos = false,
        [FromQuery] int? categoriaId = null,
        CancellationToken cancellationToken = default)
    {
        var input = new ListProdutosInput { ApenasAtivos = apenasAtivos, CategoriaId = categoriaId };
        var result = await _listProdutos.ExecuteAsync(input, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Busca um produto por ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var input = new GetProdutoInput(id);
        var result = await _getProduto.ExecuteAsync(input, cancellationToken);

        if (!result.Success)
            return NotFound(new { result.ErrorMessage });

        return Ok(result.Produto);
    }

    /// <summary>
    /// Cria um novo produto
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProdutoInput input, CancellationToken cancellationToken = default)
    {
        // Validação
        var validationResult = await _createValidator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                Errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            });
        }

        var result = await _createProduto.ExecuteAsync(input, cancellationToken);

        if (!result.Success)
            return BadRequest(new { result.ErrorMessage });

        return CreatedAtAction(nameof(GetById), new { id = result.Produto!.Id }, result.Produto);
    }
}
