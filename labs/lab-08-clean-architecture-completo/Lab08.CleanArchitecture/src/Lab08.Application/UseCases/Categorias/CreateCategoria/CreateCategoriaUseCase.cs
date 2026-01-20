using Lab08.Application.DTOs;
using Lab08.Application.Interfaces;
using Lab08.Domain.Entities;
using Lab08.Domain.Exceptions;
using Lab08.Domain.Interfaces;

namespace Lab08.Application.UseCases.Categorias.CreateCategoria;

/// <summary>
/// Input para criação de categoria
/// </summary>
public record CreateCategoriaInput
{
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
}

/// <summary>
/// Output da criação de categoria
/// </summary>
public record CreateCategoriaOutput
{
    public bool Success { get; init; }
    public CategoriaDto? Categoria { get; init; }
    public string? ErrorMessage { get; init; }

    public static CreateCategoriaOutput Ok(CategoriaDto categoria)
        => new() { Success = true, Categoria = categoria };

    public static CreateCategoriaOutput Error(string message)
        => new() { Success = false, ErrorMessage = message };
}

/// <summary>
/// Use Case para criar uma nova categoria
/// </summary>
public class CreateCategoriaUseCase : IUseCase<CreateCategoriaInput, CreateCategoriaOutput>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoriaUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateCategoriaOutput> ExecuteAsync(CreateCategoriaInput input, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validar unicidade do nome
            if (await _unitOfWork.Categorias.ExisteNomeAsync(input.Nome, cancellationToken: cancellationToken))
                return CreateCategoriaOutput.Error("Já existe uma categoria com este nome");

            // Criar categoria
            var categoria = new Categoria(input.Nome, input.Descricao);

            // Persistir
            await _unitOfWork.Categorias.AddAsync(categoria, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Mapear para DTO
            var dto = new CategoriaDto
            {
                Id = categoria.Id,
                Nome = categoria.Nome,
                Descricao = categoria.Descricao,
                Ativo = categoria.Ativo,
                QuantidadeProdutos = 0
            };

            return CreateCategoriaOutput.Ok(dto);
        }
        catch (DomainException ex)
        {
            return CreateCategoriaOutput.Error(ex.Message);
        }
    }
}
