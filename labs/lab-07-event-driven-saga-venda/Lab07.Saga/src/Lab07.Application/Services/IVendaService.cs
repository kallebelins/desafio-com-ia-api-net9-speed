using Lab07.Core.ValueObjects;

namespace Lab07.Application.Services;

/// <summary>
/// Serviço de gestão de vendas
/// </summary>
public interface IVendaService
{
    Task<VendaDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<VendaDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<VendaDto>> GetByClienteAsync(Guid clienteId, CancellationToken cancellationToken = default);
    Task<VendaDto?> CriarVendaAsync(Guid vendaId, Guid clienteId, List<ItemVendaDto> itens, decimal valorTotal, CancellationToken cancellationToken = default);
    Task<bool> ConfirmarVendaAsync(Guid vendaId, CancellationToken cancellationToken = default);
    Task<bool> CancelarVendaAsync(Guid vendaId, string motivo, CancellationToken cancellationToken = default);
}
