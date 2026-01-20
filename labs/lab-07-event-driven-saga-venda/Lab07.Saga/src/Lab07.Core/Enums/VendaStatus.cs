namespace Lab07.Core.Enums;

/// <summary>
/// Status da venda no ciclo de vida
/// </summary>
public enum VendaStatus
{
    Pendente = 0,
    Processando = 1,
    EstoqueReservado = 2,
    Confirmada = 3,
    Cancelada = 4,
    Falha = 5
}
