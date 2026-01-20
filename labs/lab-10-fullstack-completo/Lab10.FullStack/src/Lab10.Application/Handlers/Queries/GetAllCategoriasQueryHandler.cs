using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Core.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;
using Lab10.Application.DTOs;
using Lab10.Application.Queries.Categorias;
using Lab10.Domain.Interfaces;

namespace Lab10.Application.Handlers.Queries;

public class GetAllCategoriasQueryHandler : IMediatorQueryHandler<GetAllCategoriasQuery, IBusinessResult<IEnumerable<CategoriaDto>>>
{
    private readonly ICategoriaRepository _categoriaRepository;

    public GetAllCategoriasQueryHandler(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public async Task<IBusinessResult<IEnumerable<CategoriaDto>>> Handle(GetAllCategoriasQuery request, CancellationToken cancellationToken)
    {
        var categorias = request.ApenasAtivas
            ? await _categoriaRepository.GetAtivasAsync(cancellationToken)
            : await _categoriaRepository.GetAllAsync(cancellationToken);

        var dtos = categorias.Select(c => new CategoriaDto(c.Id, c.Nome, c.Descricao, c.Ativo));

        return new BusinessResult<IEnumerable<CategoriaDto>>(dtos);
    }
}
