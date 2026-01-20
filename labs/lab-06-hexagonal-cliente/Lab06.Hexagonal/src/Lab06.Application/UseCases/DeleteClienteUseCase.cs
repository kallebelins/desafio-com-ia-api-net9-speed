using Lab06.Application.Ports.Inbound;
using Lab06.Application.Ports.Outbound;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Core.ValueObjects.Logic;

namespace Lab06.Application.UseCases;

/// <summary>
/// Implementação do Use Case de exclusão de cliente
/// </summary>
public class DeleteClienteUseCase : IDeleteClienteUseCase
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IEmailService _emailService;

    public DeleteClienteUseCase(
        IClienteRepository clienteRepository,
        IEmailService emailService)
    {
        _clienteRepository = clienteRepository;
        _emailService = emailService;
    }

    public async Task<IBusinessResult<bool>> ExecuteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        // 1. Buscar cliente
        var cliente = await _clienteRepository.GetByIdAsync(id, cancellationToken);
        if (cliente == null)
        {
            return CreateErrorResult<bool>("Cliente não encontrado");
        }

        // 2. Guardar dados para notificação
        var email = cliente.Email.Value;
        var nome = cliente.Nome;

        // 3. Deletar cliente
        await _clienteRepository.DeleteAsync(cliente, cancellationToken);
        await _clienteRepository.SaveChangesAsync(cancellationToken);

        // 4. Enviar notificação de desativação (fire and forget)
        _ = _emailService.SendDeactivationEmailAsync(email, nome, cancellationToken);

        return new BusinessResult<bool>(true);
    }

    private static IBusinessResult<T> CreateErrorResult<T>(string message)
    {
        IReadOnlyCollection<IMessageResult> messages = new List<IMessageResult>
        {
            new MessageResult(message, MessageType.Error)
        };
        return new BusinessResult<T>(default!, messages);
    }
}
