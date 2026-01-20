namespace Lab10.Application.Interfaces;

/// <summary>
/// Interface do Unit of Work para a camada de aplicação
/// </summary>
public interface IUnitOfWorkApplication
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
