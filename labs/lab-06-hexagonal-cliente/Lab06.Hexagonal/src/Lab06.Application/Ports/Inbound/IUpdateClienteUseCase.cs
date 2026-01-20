using Lab06.Application.DTOs.Requests;
using Lab06.Application.DTOs.Responses;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;

namespace Lab06.Application.Ports.Inbound;

/// <summary>
/// Inbound Port (Driving) - Use Case para atualizar cliente
/// </summary>
public interface IUpdateClienteUseCase
{
    Task<IBusinessResult<ClienteResponse>> ExecuteAsync(
        int id,
        UpdateClienteRequest request,
        CancellationToken cancellationToken = default);
}
