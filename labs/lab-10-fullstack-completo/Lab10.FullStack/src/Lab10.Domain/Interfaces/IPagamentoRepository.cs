using Lab10.Domain.Entities;
using Lab10.Domain.Enums;

namespace Lab10.Domain.Interfaces;

/// <summary>
/// Interface do repositório de Pagamento
/// </summary>
public interface IPagamentoRepository
{
    Task<Pagamento?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Pagamento?> GetByVendaIdAsync(int vendaId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Pagamento>> GetByStatusAsync(PagamentoStatus status, CancellationToken cancellationToken = default);
    Task AddAsync(Pagamento pagamento, CancellationToken cancellationToken = default);
    Task UpdateAsync(Pagamento pagamento, CancellationToken cancellationToken = default);
}
