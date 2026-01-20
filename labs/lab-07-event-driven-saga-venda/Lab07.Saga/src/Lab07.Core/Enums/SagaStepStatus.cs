namespace Lab07.Core.Enums;

/// <summary>
/// Status de um step da saga
/// </summary>
public enum SagaStepStatus
{
    Pendente = 0,
    Executando = 1,
    Concluido = 2,
    Falha = 3,
    Compensando = 4,
    Compensado = 5
}
