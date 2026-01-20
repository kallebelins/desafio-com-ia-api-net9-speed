namespace Lab07.Core.Enums;

/// <summary>
/// Status geral da saga
/// </summary>
public enum SagaStatus
{
    NaoIniciada = 0,
    Executando = 1,
    Concluida = 2,
    Falha = 3,
    Compensando = 4,
    Compensada = 5
}
