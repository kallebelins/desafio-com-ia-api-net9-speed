using Lab04.Core.ValueObjects;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;

namespace Lab04.Core.Contract.Services;

/// <summary>
/// Interface do serviço de clientes
/// </summary>
public interface IClienteService
{
    Task<IBusinessResult<IEnumerable<ClienteDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IBusinessResult<ClienteDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IBusinessResult<ClienteDto>> CreateAsync(ClienteCreateDto dto, CancellationToken cancellationToken = default);
    Task<IBusinessResult<ClienteDto>> UpdateAsync(int id, ClienteUpdateDto dto, CancellationToken cancellationToken = default);
    Task<IBusinessResult<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
