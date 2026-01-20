using Lab08.Application.DTOs;
using Lab08.Application.Interfaces;
using Lab08.Domain.Entities;
using Lab08.Domain.Interfaces;

namespace Lab08.Application.UseCases.Clientes.ListClientes;

/// <summary>
/// Input para listar clientes
/// </summary>
public record ListClientesInput
{
    public bool ApenasAtivos { get; init; } = false;
}

/// <summary>
/// Output da listagem de clientes
/// </summary>
public record ListClientesOutput
{
    public IReadOnlyList<ClienteResumoDto> Clientes { get; init; } = [];
    public int Total { get; init; }
}

/// <summary>
/// Use Case para listar clientes
/// </summary>
public class ListClientesUseCase : IUseCase<ListClientesInput, ListClientesOutput>
{
    private readonly IUnitOfWork _unitOfWork;

    public ListClientesUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ListClientesOutput> ExecuteAsync(ListClientesInput input, CancellationToken cancellationToken = default)
    {
        var clientes = input.ApenasAtivos
            ? await _unitOfWork.Clientes.GetAtivosAsync(cancellationToken)
            : await _unitOfWork.Clientes.GetAllAsync(cancellationToken);

        var dtos = clientes.Select(MapToResumoDto).ToList();

        return new ListClientesOutput
        {
            Clientes = dtos,
            Total = dtos.Count
        };
    }

    private static ClienteResumoDto MapToResumoDto(Cliente cliente)
    {
        return new ClienteResumoDto
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            Email = cliente.Email.Valor,
            Ativo = cliente.Ativo
        };
    }
}
