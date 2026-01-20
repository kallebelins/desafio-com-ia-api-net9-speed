using FluentValidation;
using Lab03.Application.Commands;
using Lab03.Application.Queries;
using Lab03.Core.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;

namespace Lab03.WebAPI.Controllers;

/// <summary>
/// Controller para gerenciamento de Produtos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProdutoController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IValidator<CreateProdutoCommand>? _createValidator;
    private readonly IValidator<UpdateProdutoCommand>? _updateValidator;

    public ProdutoController(
        ISender sender,
        IValidator<CreateProdutoCommand>? createValidator = null,
        IValidator<UpdateProdutoCommand>? updateValidator = null)
    {
        _sender = sender;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <summary>
    /// Obtém todos os produtos
    /// </summary>
    /// <param name="apenasAtivos">Filtrar apenas produtos ativos</param>
    /// <param name="categoria">Filtrar por categoria</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de produtos</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProdutoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetAll(
        [FromQuery] bool? apenasAtivos = true,
        [FromQuery] string? categoria = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllProdutosQuery(apenasAtivos, categoria);
        var result = await _sender.SendAsync(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Obtém um produto por ID
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Produto encontrado ou NotFound</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProdutoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoDto>> GetById(
        int id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProdutoByIdQuery(id);
        var result = await _sender.SendAsync(query, cancellationToken);

        if (result is null)
            return NotFound(new { Message = $"Produto com ID {id} não encontrado" });

        return Ok(result);
    }

    /// <summary>
    /// Cria um novo produto
    /// </summary>
    /// <param name="command">Dados do novo produto</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Produto criado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ProdutoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProdutoDto>> Create(
        [FromBody] CreateProdutoCommand command,
        CancellationToken cancellationToken = default)
    {
        // Validação manual
        if (_createValidator is not null)
        {
            var validationResult = await _createValidator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return BadRequest(new 
                { 
                    Errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) 
                });
            }
        }

        var result = await _sender.SendAsync(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Atualiza um produto existente
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <param name="command">Dados atualizados do produto</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Produto atualizado ou NotFound</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProdutoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoDto>> Update(
        int id,
        [FromBody] UpdateProdutoRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateProdutoCommand(
            id,
            request.Nome,
            request.Descricao,
            request.Preco,
            request.Categoria,
            request.Estoque,
            request.Ativo
        );

        // Validação manual
        if (_updateValidator is not null)
        {
            var validationResult = await _updateValidator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return BadRequest(new 
                { 
                    Errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) 
                });
            }
        }

        var result = await _sender.SendAsync(command, cancellationToken);

        if (result is null)
            return NotFound(new { Message = $"Produto com ID {id} não encontrado" });

        return Ok(result);
    }

    /// <summary>
    /// Exclui um produto
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>NoContent se excluído ou NotFound</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(
        int id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteProdutoCommand(id);
        var result = await _sender.SendAsync(command, cancellationToken);

        if (!result)
            return NotFound(new { Message = $"Produto com ID {id} não encontrado" });

        return NoContent();
    }
}

/// <summary>
/// Request model para atualização de produto
/// </summary>
public record UpdateProdutoRequest(
    string Nome,
    string? Descricao,
    decimal Preco,
    string Categoria,
    int Estoque,
    bool Ativo
);
