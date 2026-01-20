using Lab07.Application.Sagas;
using Lab07.Application.Services;
using Lab07.Core.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace Lab07.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendaController : ControllerBase
{
    private readonly IVendaService _vendaService;
    private readonly CriarVendaSaga _criarVendaSaga;
    private readonly ILogger<VendaController> _logger;

    public VendaController(
        IVendaService vendaService,
        CriarVendaSaga criarVendaSaga,
        ILogger<VendaController> logger)
    {
        _vendaService = vendaService;
        _criarVendaSaga = criarVendaSaga;
        _logger = logger;
    }

    /// <summary>
    /// Lista todas as vendas
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VendaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VendaDto>>> GetAll(CancellationToken cancellationToken)
    {
        var vendas = await _vendaService.GetAllAsync(cancellationToken);
        return Ok(vendas);
    }

    /// <summary>
    /// Obtém uma venda pelo ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(VendaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendaDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var venda = await _vendaService.GetByIdAsync(id, cancellationToken);
        
        if (venda == null)
            return NotFound();

        return Ok(venda);
    }

    /// <summary>
    /// Lista vendas de um cliente
    /// </summary>
    [HttpGet("cliente/{clienteId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<VendaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VendaDto>>> GetByCliente(Guid clienteId, CancellationToken cancellationToken)
    {
        var vendas = await _vendaService.GetByClienteAsync(clienteId, cancellationToken);
        return Ok(vendas);
    }

    /// <summary>
    /// Cria uma nova venda usando o Saga Pattern
    /// </summary>
    /// <remarks>
    /// Este endpoint executa a saga de criação de venda que inclui:
    /// 1. Validar cliente
    /// 2. Validar produtos
    /// 3. Reservar estoque
    /// 4. Criar venda
    /// 5. Notificar cliente
    /// 
    /// Em caso de falha, todas as operações são compensadas automaticamente.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(CriarVendaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CriarVendaResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CriarVendaResponse>> Create([FromBody] CriarVendaRequest request, CancellationToken cancellationToken)
    {
        if (request.ClienteId == Guid.Empty)
            return BadRequest(new CriarVendaResponse
            {
                Success = false,
                ErrorMessage = "ClienteId é obrigatório"
            });

        if (request.Itens == null || request.Itens.Count == 0)
            return BadRequest(new CriarVendaResponse
            {
                Success = false,
                ErrorMessage = "A venda deve ter pelo menos um item"
            });

        _logger.LogInformation(
            "Iniciando saga de criação de venda para cliente {ClienteId} com {TotalItens} itens",
            request.ClienteId, request.Itens.Count);

        var result = await _criarVendaSaga.ExecuteAsync(request, cancellationToken);

        var response = new CriarVendaResponse
        {
            SagaId = result.SagaId,
            Success = result.IsSuccess,
            ErrorMessage = result.ErrorMessage,
            Venda = result.Data
        };

        if (result.IsSuccess)
        {
            _logger.LogInformation(
                "Venda {VendaId} criada com sucesso via saga {SagaId}",
                result.Data?.Id, result.SagaId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Data?.Id },
                response);
        }

        _logger.LogWarning(
            "Saga {SagaId} falhou: {ErrorMessage}",
            result.SagaId, result.ErrorMessage);

        return BadRequest(response);
    }

    /// <summary>
    /// Cancela uma venda
    /// </summary>
    [HttpPatch("{id:guid}/cancelar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancelar(Guid id, [FromBody] CancelarVendaRequest request, CancellationToken cancellationToken)
    {
        var success = await _vendaService.CancelarVendaAsync(
            id,
            request.Motivo ?? "Cancelado pelo usuário",
            cancellationToken);
        
        if (!success)
            return NotFound();

        return NoContent();
    }
}

public record CriarVendaResponse
{
    public Guid SagaId { get; init; }
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public VendaDto? Venda { get; init; }
}

public record CancelarVendaRequest(string? Motivo);
