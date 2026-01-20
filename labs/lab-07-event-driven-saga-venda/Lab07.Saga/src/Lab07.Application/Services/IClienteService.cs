using Lab07.Core.ValueObjects;

namespace Lab07.Application.Services;

/// <summary>
/// Serviço de gestão de clientes
/// </summary>
public interface IClienteService
{
    Task<ClienteDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ClienteDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ClienteDto> CreateAsync(string nome, string email, string telefone, CancellationToken cancellationToken = default);
    Task<ClienteDto?> UpdateAsync(Guid id, string nome, string email, string telefone, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> AtivarAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> DesativarAsync(Guid id, CancellationToken cancellationToken = default);
}
