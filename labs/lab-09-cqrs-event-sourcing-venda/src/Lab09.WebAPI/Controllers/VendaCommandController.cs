using Lab09.Application.Commands;
using Lab09.Application.DTOs;
using Lab09.Application.Handlers.Commands;
using Lab09.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Lab09.WebAPI.Controllers;

/// <summary>
/// Controller para operações de escrita (Commands) de Venda
/// </summary>
[ApiController]
[Route("api/vendas")]
[Produces("application/json")]
public class VendaCommandController : ControllerBase
{
    private readonly VendaCommandHandler _handler;
    private readonly ILogger<VendaCommandController> _logger;

    public VendaCommandController(
        VendaCommandHandler handler,
        ILogger<VendaCommandController> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    /// <summary>
    /// Inicia uma nova venda
    /// </summary>
    /// <param name="command">Dados para iniciar a venda</param>
    /// <returns>Venda criada</returns>
    [HttpPost]
    [ProducesResponseType(typeof(VendaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VendaDto>> IniciarVenda(
        [FromBody] IniciarVendaCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Iniciando nova venda para cliente {ClienteId}", command.ClienteId);
            
            var result = await _handler.Handle(command, cancellationToken);
            
            _logger.LogInformation("Venda {VendaId} iniciada com sucesso", result.Id);
            
            return CreatedAtAction(
                actionName: nameof(VendaQueryController.GetById),
                controllerName: "VendaQuery",
                routeValues: new { id = result.Id },
                value: result);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao iniciar venda");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Adiciona um item à venda
    /// </summary>
    /// <param name="id">ID da venda</param>
    /// <param name="request">Dados do item</param>
    /// <returns>Venda atualizada</returns>
    [HttpPost("{id:guid}/itens")]
    [ProducesResponseType(typeof(VendaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendaDto>> AdicionarItem(
        Guid id,
        [FromBody] AdicionarItemRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new AdicionarItemCommand
            {
                VendaId = id,
                ProdutoId = request.ProdutoId,
                ProdutoNome = request.ProdutoNome,
                Quantidade = request.Quantidade,
                PrecoUnitario = request.PrecoUnitario
            };

            _logger.LogInformation("Adicionando item {ProdutoId} à venda {VendaId}", 
                request.ProdutoId, id);

            var result = await _handler.Handle(command, cancellationToken);

            return Ok(result);
        }
        catch (DomainException ex) when (ex.Message.Contains("não encontrada"))
        {
            return NotFound(new { error = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Remove um item da venda
    /// </summary>
    /// <param name="id">ID da venda</param>
    /// <param name="produtoId">ID do produto a remover</param>
    /// <returns>Venda atualizada</returns>
    [HttpDelete("{id:guid}/itens/{produtoId:guid}")]
    [ProducesResponseType(typeof(VendaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendaDto>> RemoverItem(
        Guid id,
        Guid produtoId,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new RemoverItemCommand
            {
                VendaId = id,
                ProdutoId = produtoId
            };

            _logger.LogInformation("Removendo item {ProdutoId} da venda {VendaId}", 
                produtoId, id);

            var result = await _handler.Handle(command, cancellationToken);

            return Ok(result);
        }
        catch (DomainException ex) when (ex.Message.Contains("não encontrada"))
        {
            return NotFound(new { error = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Aplica desconto à venda
    /// </summary>
    /// <param name="id">ID da venda</param>
    /// <param name="request">Dados do desconto</param>
    /// <returns>Venda atualizada</returns>
    [HttpPost("{id:guid}/desconto")]
    [ProducesResponseType(typeof(VendaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendaDto>> AplicarDesconto(
        Guid id,
        [FromBody] AplicarDescontoRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new AplicarDescontoCommand
            {
                VendaId = id,
                ValorDesconto = request.ValorDesconto,
                PercentualDesconto = request.PercentualDesconto,
                Motivo = request.Motivo
            };

            _logger.LogInformation("Aplicando desconto à venda {VendaId}", id);

            var result = await _handler.Handle(command, cancellationToken);

            return Ok(result);
        }
        catch (DomainException ex) when (ex.Message.Contains("não encontrada"))
        {
            return NotFound(new { error = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Finaliza a venda
    /// </summary>
    /// <param name="id">ID da venda</param>
    /// <returns>Venda finalizada</returns>
    [HttpPost("{id:guid}/finalizar")]
    [ProducesResponseType(typeof(VendaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendaDto>> FinalizarVenda(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new FinalizarVendaCommand { VendaId = id };

            _logger.LogInformation("Finalizando venda {VendaId}", id);

            var result = await _handler.Handle(command, cancellationToken);

            _logger.LogInformation("Venda {VendaId} finalizada. Total: {Total}", 
                id, result.Total);

            return Ok(result);
        }
        catch (DomainException ex) when (ex.Message.Contains("não encontrada"))
        {
            return NotFound(new { error = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Cancela a venda
    /// </summary>
    /// <param name="id">ID da venda</param>
    /// <param name="request">Motivo do cancelamento</param>
    /// <returns>Venda cancelada</returns>
    [HttpPost("{id:guid}/cancelar")]
    [ProducesResponseType(typeof(VendaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendaDto>> CancelarVenda(
        Guid id,
        [FromBody] CancelarVendaRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CancelarVendaCommand
            {
                VendaId = id,
                Motivo = request.Motivo
            };

            _logger.LogInformation("Cancelando venda {VendaId}. Motivo: {Motivo}", 
                id, request.Motivo);

            var result = await _handler.Handle(command, cancellationToken);

            return Ok(result);
        }
        catch (DomainException ex) when (ex.Message.Contains("não encontrada"))
        {
            return NotFound(new { error = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

// Request DTOs
public record AdicionarItemRequest
{
    public Guid ProdutoId { get; init; }
    public string ProdutoNome { get; init; } = string.Empty;
    public int Quantidade { get; init; }
    public decimal PrecoUnitario { get; init; }
}

public record AplicarDescontoRequest
{
    public decimal? ValorDesconto { get; init; }
    public decimal? PercentualDesconto { get; init; }
    public string? Motivo { get; init; }
}

public record CancelarVendaRequest
{
    public string Motivo { get; init; } = string.Empty;
}
