using Lab08.Domain.Entities;
using Lab08.Domain.Enums;

namespace Lab08.Domain.Interfaces;

/// <summary>
/// Interface de repositório para Venda
/// </summary>
public interface IVendaRepository
{
    Task<Venda?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Venda?> GetByIdWithItensAsync(int id, CancellationToken cancellationToken = default);
    Task<Venda?> GetByIdCompletoAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Venda>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Venda>> GetByClienteAsync(int clienteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Venda>> GetByStatusAsync(StatusVenda status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Venda>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Venda>> GetByPeriodoComItensAsync(DateTime dataInicio, DateTime dataFim, CancellationToken cancellationToken = default);
    Task AddAsync(Venda venda, CancellationToken cancellationToken = default);
    Task UpdateAsync(Venda venda, CancellationToken cancellationToken = default);
}
