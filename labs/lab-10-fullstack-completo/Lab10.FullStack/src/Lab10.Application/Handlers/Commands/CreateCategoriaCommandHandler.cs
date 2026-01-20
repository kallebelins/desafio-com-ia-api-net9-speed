using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Core.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Cqrs.Abstractions;
using Microsoft.Extensions.Logging;
using Lab10.Application.Commands.Categorias;
using Lab10.Application.DTOs;
using Lab10.Application.Interfaces;
using Lab10.Domain.Entities;
using Lab10.Domain.Interfaces;

namespace Lab10.Application.Handlers.Commands;

public class CreateCategoriaCommandHandler : IMediatorCommandHandler<CreateCategoriaCommand, IBusinessResult<CategoriaDto>>
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IUnitOfWorkApplication _unitOfWork;
    private readonly ILogger<CreateCategoriaCommandHandler> _logger;

    public CreateCategoriaCommandHandler(
        ICategoriaRepository categoriaRepository,
        IUnitOfWorkApplication unitOfWork,
        ILogger<CreateCategoriaCommandHandler> logger)
    {
        _categoriaRepository = categoriaRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IBusinessResult<CategoriaDto>> Handle(CreateCategoriaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Criando categoria: {Nome}", request.Nome);

            // Verificar se já existe categoria com mesmo nome
            var existente = await _categoriaRepository.GetByNomeAsync(request.Nome, cancellationToken);
            if (existente != null)
                return CreateErrorResult<CategoriaDto>("Já existe uma categoria com este nome");

            // Criar entidade
            var categoria = new Categoria(request.Nome, request.Descricao);
            
            await _categoriaRepository.AddAsync(categoria, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Retornar DTO
            var dto = new CategoriaDto(categoria.Id, categoria.Nome, categoria.Descricao, categoria.Ativo);
            
            _logger.LogInformation("Categoria criada com sucesso: {Id}", categoria.Id);
            return new BusinessResult<CategoriaDto>(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar categoria");
            return CreateErrorResult<CategoriaDto>("Erro interno ao criar categoria");
        }
    }

    private static IBusinessResult<T> CreateErrorResult<T>(string message)
    {
        IReadOnlyCollection<IMessageResult> messages = new List<IMessageResult>
        {
            new MessageResult(message, MessageType.Error)
        };
        return new BusinessResult<T>(default!, messages);
    }
}
