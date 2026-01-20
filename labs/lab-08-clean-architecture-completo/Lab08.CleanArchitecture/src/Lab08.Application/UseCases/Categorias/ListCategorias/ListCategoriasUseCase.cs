using Lab08.Application.DTOs;
using Lab08.Application.Interfaces;
using Lab08.Domain.Interfaces;

namespace Lab08.Application.UseCases.Categorias.ListCategorias;

/// <summary>
/// Input para listar categorias
/// </summary>
public record ListCategoriasInput
{
    public bool ApenasAtivas { get; init; } = false;
}

/// <summary>
/// Output da listagem de categorias
/// </summary>
public record ListCategoriasOutput
{
    public IReadOnlyList<CategoriaDto> Categorias { get; init; } = [];
    public int Total { get; init; }
}

/// <summary>
/// Use Case para listar categorias
/// </summary>
public class ListCategoriasUseCase : IUseCase<ListCategoriasInput, ListCategoriasOutput>
{
    private readonly IUnitOfWork _unitOfWork;

    public ListCategoriasUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ListCategoriasOutput> ExecuteAsync(ListCategoriasInput input, CancellationToken cancellationToken = default)
    {
        var categorias = input.ApenasAtivas
            ? await _unitOfWork.Categorias.GetAtivasAsync(cancellationToken)
            : await _unitOfWork.Categorias.GetAllAsync(cancellationToken);

        var dtos = categorias.Select(c => new CategoriaDto
        {
            Id = c.Id,
            Nome = c.Nome,
            Descricao = c.Descricao,
            Ativo = c.Ativo,
            QuantidadeProdutos = c.Produtos?.Count ?? 0
        }).ToList();

        return new ListCategoriasOutput
        {
            Categorias = dtos,
            Total = dtos.Count
        };
    }
}
