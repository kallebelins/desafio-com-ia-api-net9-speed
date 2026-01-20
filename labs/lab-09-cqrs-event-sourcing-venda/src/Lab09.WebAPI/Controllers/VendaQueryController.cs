using Lab09.Application.DTOs;
using Lab09.Application.Handlers.Queries;
using Lab09.Application.Projections;
using Lab09.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Lab09.WebAPI.Controllers;

/// <summary>
/// Controller para operações de leitura (Queries) de Venda
/// </summary>
[ApiController]
[Route("api/vendas")]
[Produces("application/json")]
public class VendaQueryController : ControllerBase
{
    private readonly VendaQueryHandler _handler;
    private readonly ILogger<VendaQueryController> _logger;

    public VendaQueryController(
        VendaQueryHandler handler,
        ILogger<VendaQueryController> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    /// <summary>
    /// Obtém uma venda por ID (reconstrói do Event Store)
    /// </summary>
    /// <param name="id">ID da venda</param>
    /// <returns>Venda encontrada</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(VendaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendaDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando venda {VendaId}", id);

        var query = new GetVendaByIdQuery { VendaId = id };
        var result = await _handler.Handle(query, cancellationToken);

        if (result == null)
        {
            return NotFound(new { error = $"Venda {id} não encontrada" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtém o histórico completo de eventos de uma venda
    /// </summary>
    /// <param name="id">ID da venda</param>
    /// <returns>Histórico de eventos</returns>
    [HttpGet("{id:guid}/historico")]
    [ProducesResponseType(typeof(VendaHistoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendaHistoryDto>> GetHistory(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando histórico da venda {VendaId}", id);

        var query = new GetVendaHistoryQuery { VendaId = id };
        var result = await _handler.Handle(query, cancellationToken);

        if (result == null)
        {
            return NotFound(new { error = $"Venda {id} não encontrada" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Reconstrói o estado da venda em um momento específico (Time Travel)
    /// </summary>
    /// <param name="id">ID da venda</param>
    /// <param name="momento">Data/hora para reconstruir o estado</param>
    /// <returns>Estado da venda no momento especificado</returns>
    [HttpGet("{id:guid}/momento")]
    [ProducesResponseType(typeof(VendaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendaDto>> GetAtMoment(
        Guid id,
        [FromQuery] DateTime momento,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando estado da venda {VendaId} no momento {Momento}", 
            id, momento);

        var query = new GetVendaAtMomentQuery
        {
            VendaId = id,
            Momento = momento
        };
        var result = await _handler.Handle(query, cancellationToken);

        if (result == null)
        {
            return NotFound(new { error = $"Venda {id} não encontrada ou sem eventos até {momento}" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Lista vendas por período (consulta no Read Model)
    /// </summary>
    /// <param name="dataInicio">Data de início do período</param>
    /// <param name="dataFim">Data de fim do período</param>
    /// <param name="status">Status da venda (opcional)</param>
    /// <param name="page">Número da página</param>
    /// <param name="pageSize">Tamanho da página</param>
    /// <returns>Lista de vendas</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VendaReadModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VendaReadModel>>> GetByPeriodo(
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Buscando vendas. Período: {DataInicio} a {DataFim}, Status: {Status}",
            dataInicio, dataFim, status);

        var query = new GetVendasPorPeriodoQuery
        {
            DataInicio = dataInicio,
            DataFim = dataFim,
            Status = status,
            Page = page,
            PageSize = pageSize
        };

        var result = await _handler.Handle(query, cancellationToken);

        return Ok(result);
    }
}
