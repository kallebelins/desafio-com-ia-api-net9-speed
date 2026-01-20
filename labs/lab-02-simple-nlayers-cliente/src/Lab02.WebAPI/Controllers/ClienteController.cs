using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Mvp24Hours.Core.Contract.Data;
using Lab02.Core.Entities;
using Lab02.Core.ValueObjects;

namespace Lab02.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClienteController : ControllerBase
{
    private readonly IUnitOfWorkAsync _uow;
    private readonly IValidator<ClienteCreateDto> _createValidator;
    private readonly IValidator<ClienteUpdateDto> _updateValidator;

    public ClienteController(
        IUnitOfWorkAsync uow,
        IValidator<ClienteCreateDto> createValidator,
        IValidator<ClienteUpdateDto> updateValidator)
    {
        _uow = uow;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var repository = _uow.GetRepository<Cliente>();
        var clientes = await repository.ListAsync();
        var clientesDto = clientes.Select(c => new ClienteDto(
            c.Id,
            c.Nome,
            c.Email,
            c.Telefone,
            c.Ativo,
            c.DataCriacao
        )).ToList();
        return Ok(clientesDto);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var repository = _uow.GetRepository<Cliente>();
        var cliente = await repository.GetByIdAsync(id);
        
        if (cliente == null)
            return NotFound();
            
        var clienteDto = new ClienteDto(
            cliente.Id,
            cliente.Nome,
            cliente.Email,
            cliente.Telefone,
            cliente.Ativo,
            cliente.DataCriacao
        );
        return Ok(clienteDto);
    }

    [HttpGet("email/{email}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByEmail(string email)
    {
        var repository = _uow.GetRepository<Cliente>();
        var clientes = await repository.GetByAsync(c => c.Email == email);
        var cliente = clientes.FirstOrDefault();
        
        if (cliente == null)
            return NotFound();
            
        var clienteDto = new ClienteDto(
            cliente.Id,
            cliente.Nome,
            cliente.Email,
            cliente.Telefone,
            cliente.Ativo,
            cliente.DataCriacao
        );
        return Ok(clienteDto);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] ClienteCreateDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));

        // Validação de email único
        var repository = _uow.GetRepository<Cliente>();
        var emailExiste = await repository.GetByAnyAsync(c => c.Email == dto.Email);
        if (emailExiste)
            return BadRequest(new { Message = "Email já cadastrado" });

        var cliente = new Cliente
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Telefone = dto.Telefone,
            Ativo = true,
            DataCriacao = DateTime.UtcNow
        };

        await repository.AddAsync(cliente);
        await _uow.SaveChangesAsync();

        var clienteDto = new ClienteDto(
            cliente.Id,
            cliente.Nome,
            cliente.Email,
            cliente.Telefone,
            cliente.Ativo,
            cliente.DataCriacao
        );
        return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, clienteDto);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] ClienteUpdateDto dto)
    {
        var repository = _uow.GetRepository<Cliente>();
        var cliente = await repository.GetByIdAsync(id);
        
        if (cliente == null)
            return NotFound();

        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));

        // Validação de email único (excluindo o próprio registro)
        var emailExiste = await repository.GetByAnyAsync(c => c.Email == dto.Email && c.Id != id);
        if (emailExiste)
            return BadRequest(new { Message = "Email já cadastrado" });

        cliente.Nome = dto.Nome;
        cliente.Email = dto.Email;
        cliente.Telefone = dto.Telefone;
        cliente.Ativo = dto.Ativo;

        await repository.ModifyAsync(cliente);
        await _uow.SaveChangesAsync();

        var clienteDto = new ClienteDto(
            cliente.Id,
            cliente.Nome,
            cliente.Email,
            cliente.Telefone,
            cliente.Ativo,
            cliente.DataCriacao
        );
        return Ok(clienteDto);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var repository = _uow.GetRepository<Cliente>();
        var cliente = await repository.GetByIdAsync(id);
        
        if (cliente == null)
            return NotFound();

        await repository.RemoveAsync(cliente);
        await _uow.SaveChangesAsync();

        return NoContent();
    }
}
