using FluentValidation;
using Lab08.Application.Interfaces;
using Lab08.Application.UseCases.Clientes.CreateCliente;
using Lab08.Application.UseCases.Clientes.GetCliente;
using Lab08.Application.UseCases.Clientes.ListClientes;
using Microsoft.AspNetCore.Mvc;

namespace Lab08.WebAPI.Controllers;

/// <summary>
/// Controller para operações de Cliente
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ClienteController : ControllerBase
{
    private readonly IUseCase<CreateClienteInput, CreateClienteOutput> _createCliente;
    private readonly IUseCase<GetClienteInput, GetClienteOutput> _getCliente;
    private readonly IUseCase<ListClientesInput, ListClientesOutput> _listClientes;
    private readonly IValidator<CreateClienteInput> _createValidator;

    public ClienteController(
        IUseCase<CreateClienteInput, CreateClienteOutput> createCliente,
        IUseCase<GetClienteInput, GetClienteOutput> getCliente,
        IUseCase<ListClientesInput, ListClientesOutput> listClientes,
        IValidator<CreateClienteInput> createValidator)
    {
        _createCliente = createCliente;
        _getCliente = getCliente;
        _listClientes = listClientes;
        _createValidator = createValidator;
    }

    /// <summary>
    /// Lista todos os clientes
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool apenasAtivos = false, CancellationToken cancellationToken = default)
    {
        var input = new ListClientesInput { ApenasAtivos = apenasAtivos };
        var result = await _listClientes.ExecuteAsync(input, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Busca um cliente por ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var input = new GetClienteInput(id);
        var result = await _getCliente.ExecuteAsync(input, cancellationToken);

        if (!result.Success)
            return NotFound(new { result.ErrorMessage });

        return Ok(result.Cliente);
    }

    /// <summary>
    /// Cria um novo cliente
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClienteInput input, CancellationToken cancellationToken = default)
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

        var result = await _createCliente.ExecuteAsync(input, cancellationToken);

        if (!result.Success)
            return BadRequest(new { result.ErrorMessage });

        return CreatedAtAction(nameof(GetById), new { id = result.Cliente!.Id }, result.Cliente);
    }
}
