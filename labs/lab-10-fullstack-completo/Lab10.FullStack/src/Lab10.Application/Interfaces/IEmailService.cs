namespace Lab10.Application.Interfaces;

/// <summary>
/// Interface do serviço de email
/// </summary>
public interface IEmailService
{
    Task EnviarEmailBoasVindasAsync(string email, string nome, CancellationToken cancellationToken = default);
    Task EnviarEmailConfirmacaoVendaAsync(string email, string nomeCliente, int vendaId, decimal valorTotal, CancellationToken cancellationToken = default);
}
