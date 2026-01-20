using Lab06.Application.DTOs.Responses;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;

namespace Lab06.Application.Ports.Inbound;

/// <summary>
/// Inbound Port (Driving) - Use Case para consultar clientes
/// </summary>
public interface IGetClienteUseCase
{
    Task<IBusinessResult<ClienteResponse>> ExecuteByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IBusinessResult<ClienteListResponse>> ExecuteAllAsync(
        CancellationToken cancellationToken = default);

    Task<IBusinessResult<ClienteListResponse>> ExecuteAllAtivosAsync(
        CancellationToken cancellationToken = default);
}
