namespace Lab07.Core.Enums;

/// <summary>
/// Status da mensagem no outbox
/// </summary>
public enum OutboxStatus
{
    Pendente = 0,
    Publicado = 1,
    Falha = 2
}
