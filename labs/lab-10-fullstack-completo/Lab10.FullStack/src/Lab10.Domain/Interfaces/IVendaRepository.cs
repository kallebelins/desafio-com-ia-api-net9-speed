using Lab10.Domain.Entities;
using Lab10.Domain.Enums;

namespace Lab10.Domain.Interfaces;

/// <summary>
/// Interface do repositório de Venda
/// </summary>
public interface IVendaRepository
{
    Task<Venda?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Venda?> GetByIdWithItensAsync(int id, CancellationToken cancellationToken = default);
    Task<Venda?> GetByIdWithItensAndPagamentoAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Venda>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Venda>> GetByClienteAsync(int clienteId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Venda>> GetByStatusAsync(VendaStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Venda>> GetByPeriodoAsync(DateTime inicio, DateTime fim, CancellationToken cancellationToken = default);
    Task AddAsync(Venda venda, CancellationToken cancellationToken = default);
    Task UpdateAsync(Venda venda, CancellationToken cancellationToken = default);

    // Relatórios
    Task<int> CountByStatusAsync(VendaStatus status, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalVendasPeriodoAsync(DateTime inicio, DateTime fim, CancellationToken cancellationToken = default);
}
