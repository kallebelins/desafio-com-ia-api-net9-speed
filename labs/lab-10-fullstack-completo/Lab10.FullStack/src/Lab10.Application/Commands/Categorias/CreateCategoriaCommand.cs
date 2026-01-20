using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Lab10.Application.DTOs;

namespace Lab10.Application.Commands.Categorias;

/// <summary>
/// Command para criar uma nova categoria
/// </summary>
public record CreateCategoriaCommand(
    string Nome,
    string? Descricao) : Mvp24Hours.Infrastructure.Cqrs.Abstractions.IMediatorCommand<IBusinessResult<CategoriaDto>>;
