using Lab06.Application.DTOs.Requests;
using Lab06.Application.DTOs.Responses;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;

namespace Lab06.Application.Ports.Inbound;

/// <summary>
/// Inbound Port (Driving) - Use Case para criar cliente
/// </summary>
public interface ICreateClienteUseCase
{
    Task<IBusinessResult<ClienteResponse>> ExecuteAsync(
        CreateClienteRequest request,
        CancellationToken cancellationToken = default);
}
