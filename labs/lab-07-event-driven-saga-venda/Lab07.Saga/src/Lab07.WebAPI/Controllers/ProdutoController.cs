using Lab07.Application.Services;
using Lab07.Core.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace Lab07.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutoController : ControllerBase
{
    private readonly IProdutoService _produtoService;
    private readonly ILogger<ProdutoController> _logger;

    public ProdutoController(
        IProdutoService produtoService,
        ILogger<ProdutoController> logger)
    {
        _produtoService = produtoService;
        _logger = logger;
    }

    /// <summary>
    /// Lista todos os produtos
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProdutoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetAll(CancellationToken cancellationToken)
    {
        var produtos = await _produtoService.GetAllAsync(cancellationToken);
        return Ok(produtos);
    }

    /// <summary>
    /// Obtém um produto pelo ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProdutoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var produto = await _produtoService.GetByIdAsync(id, cancellationToken);
        
        if (produto == null)
            return NotFound();

        return Ok(produto);
    }

    /// <summary>
    /// Cria um novo produto
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProdutoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProdutoDto>> Create([FromBody] CreateProdutoRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            return BadRequest("Nome é obrigatório");

        if (request.Preco <= 0)
            return BadRequest("Preço deve ser maior que zero");

        if (request.Estoque < 0)
            return BadRequest("Estoque não pode ser negativo");

        var produto = await _produtoService.CreateAsync(
            request.Nome,
            request.Descricao ?? string.Empty,
            request.Preco,
            request.Estoque,
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = produto.Id }, produto);
    }

    /// <summary>
    /// Atualiza um produto
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProdutoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoDto>> Update(Guid id, [FromBody] UpdateProdutoRequest request, CancellationToken cancellationToken)
    {
        var produto = await _produtoService.UpdateAsync(
            id,
            request.Nome,
            request.Descricao ?? string.Empty,
            request.Preco,
            cancellationToken);

        if (produto == null)
            return NotFound();

        return Ok(produto);
    }

    /// <summary>
    /// Remove um produto
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var success = await _produtoService.DeleteAsync(id, cancellationToken);
        
        if (!success)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Adiciona estoque a um produto
    /// </summary>
    [HttpPatch("{id:guid}/estoque/adicionar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AdicionarEstoque(Guid id, [FromBody] AlterarEstoqueRequest request, CancellationToken cancellationToken)
    {
        if (request.Quantidade <= 0)
            return BadRequest("Quantidade deve ser maior que zero");

        var success = await _produtoService.AdicionarEstoqueAsync(id, request.Quantidade, cancellationToken);
        
        if (!success)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Ativa um produto
    /// </summary>
    [HttpPatch("{id:guid}/ativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Ativar(Guid id, CancellationToken cancellationToken)
    {
        var success = await _produtoService.AtivarAsync(id, cancellationToken);
        
        if (!success)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Desativa um produto
    /// </summary>
    [HttpPatch("{id:guid}/desativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Desativar(Guid id, CancellationToken cancellationToken)
    {
        var success = await _produtoService.DesativarAsync(id, cancellationToken);
        
        if (!success)
            return NotFound();

        return NoContent();
    }
}

public record CreateProdutoRequest(string Nome, string? Descricao, decimal Preco, int Estoque);
public record UpdateProdutoRequest(string Nome, string? Descricao, decimal Preco);
public record AlterarEstoqueRequest(int Quantidade);
