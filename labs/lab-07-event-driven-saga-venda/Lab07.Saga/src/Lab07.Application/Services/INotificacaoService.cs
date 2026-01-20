namespace Lab07.Application.Services;

/// <summary>
/// Serviço de notificações (email, etc.)
/// </summary>
public interface INotificacaoService
{
    Task EnviarConfirmacaoVendaAsync(
        string email,
        string nomeCliente,
        Guid vendaId,
        decimal valorTotal,
        int totalItens,
        CancellationToken cancellationToken = default);

    Task EnviarCancelamentoVendaAsync(
        string email,
        string nomeCliente,
        Guid vendaId,
        string motivo,
        CancellationToken cancellationToken = default);
}
