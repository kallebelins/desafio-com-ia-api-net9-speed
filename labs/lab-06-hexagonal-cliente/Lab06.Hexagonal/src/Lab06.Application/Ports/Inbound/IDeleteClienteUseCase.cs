using Mvp24Hours.Core.Contract.ValueObjects.Logic;

namespace Lab06.Application.Ports.Inbound;

/// <summary>
/// Inbound Port (Driving) - Use Case para deletar cliente
/// </summary>
public interface IDeleteClienteUseCase
{
    Task<IBusinessResult<bool>> ExecuteAsync(
        int id,
        CancellationToken cancellationToken = default);
}
