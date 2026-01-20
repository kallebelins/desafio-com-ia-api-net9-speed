namespace Lab09.Core.Interfaces;

/// <summary>
/// Interface para Projeções (Read Models)
/// </summary>
public interface IProjection
{
    /// <summary>
    /// Nome da projeção
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Processa um evento e atualiza o Read Model
    /// </summary>
    /// <param name="event">Evento a ser processado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task HandleAsync(IDomainEvent @event, CancellationToken cancellationToken = default);
}
