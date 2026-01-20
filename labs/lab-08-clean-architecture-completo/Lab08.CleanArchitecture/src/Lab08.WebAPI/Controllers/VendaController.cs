using FluentValidation;
using Lab08.Application.Interfaces;
using Lab08.Application.UseCases.Vendas.ConfirmarVenda;
using Lab08.Application.UseCases.Vendas.CreateVenda;
using Lab08.Application.UseCases.Vendas.GetVenda;
using Lab08.Application.UseCases.Vendas.RelatorioVendas;
using Microsoft.AspNetCore.Mvc;

namespace Lab08.WebAPI.Controllers;

/// <summary>
/// Controller para operações de Venda
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class VendaController : ControllerBase
{
    private readonly IUseCase<CreateVendaInput, CreateVendaOutput> _createVenda;
    private readonly IUseCase<GetVendaInput, GetVendaOutput> _getVenda;
    private readonly IUseCase<ConfirmarVendaInput, ConfirmarVendaOutput> _confirmarVenda;
    private readonly IUseCase<RelatorioVendasInput, RelatorioVendasOutput> _relatorioVendas;
    private readonly IValidator<CreateVendaInput> _createValidator;

    public VendaController(
        IUseCase<CreateVendaInput, CreateVendaOutput> createVenda,
        IUseCase<GetVendaInput, GetVendaOutput> getVenda,
        IUseCase<ConfirmarVendaInput, ConfirmarVendaOutput> confirmarVenda,
        IUseCase<RelatorioVendasInput, RelatorioVendasOutput> relatorioVendas,
        IValidator<CreateVendaInput> createValidator)
    {
        _createVenda = createVenda;
        _getVenda = getVenda;
        _confirmarVenda = confirmarVenda;
        _relatorioVendas = relatorioVendas;
        _createValidator = createValidator;
    }

    /// <summary>
    /// Busca uma venda por ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var input = new GetVendaInput(id);
        var result = await _getVenda.ExecuteAsync(input, cancellationToken);

        if (!result.Success)
            return NotFound(new { result.ErrorMessage });

        return Ok(result.Venda);
    }

    /// <summary>
    /// Cria uma nova venda
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVendaInput input, CancellationToken cancellationToken = default)
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

        var result = await _createVenda.ExecuteAsync(input, cancellationToken);

        if (!result.Success)
            return BadRequest(new { result.ErrorMessage });

        return CreatedAtAction(nameof(GetById), new { id = result.Venda!.Id }, result.Venda);
    }

    /// <summary>
    /// Confirma uma venda (realiza baixa no estoque)
    /// </summary>
    [HttpPost("{id:int}/confirmar")]
    public async Task<IActionResult> Confirmar(int id, CancellationToken cancellationToken = default)
    {
        var input = new ConfirmarVendaInput(id);
        var result = await _confirmarVenda.ExecuteAsync(input, cancellationToken);

        if (!result.Success)
            return BadRequest(new { result.ErrorMessage });

        return Ok(new { result.Message });
    }

    /// <summary>
    /// Gera relatório de vendas por período
    /// </summary>
    [HttpGet("relatorio")]
    public async Task<IActionResult> Relatorio(
        [FromQuery] DateTime dataInicio,
        [FromQuery] DateTime dataFim,
        [FromQuery] int topProdutos = 10,
        CancellationToken cancellationToken = default)
    {
        var input = new RelatorioVendasInput
        {
            DataInicio = dataInicio,
            DataFim = dataFim,
            TopProdutos = topProdutos
        };

        var result = await _relatorioVendas.ExecuteAsync(input, cancellationToken);

        if (!result.Success)
            return BadRequest(new { result.ErrorMessage });

        return Ok(result.Relatorio);
    }
}
