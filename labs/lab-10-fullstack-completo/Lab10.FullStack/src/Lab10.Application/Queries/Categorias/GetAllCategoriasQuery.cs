using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Lab10.Application.DTOs;

namespace Lab10.Application.Queries.Categorias;

/// <summary>
/// Query para listar todas as categorias
/// </summary>
public record GetAllCategoriasQuery(bool ApenasAtivas = false) 
    : Mvp24Hours.Infrastructure.Cqrs.Abstractions.IMediatorQuery<IBusinessResult<IEnumerable<CategoriaDto>>>;
