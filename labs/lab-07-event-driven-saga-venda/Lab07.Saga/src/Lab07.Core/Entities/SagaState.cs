using Lab07.Core.Enums;
using Mvp24Hours.Core.Entities;

namespace Lab07.Core.Entities;

/// <summary>
/// Estado persistido de uma saga
/// </summary>
public class SagaState : EntityBase<Guid>
{
    public string SagaType { get; set; } = string.Empty;
    public SagaStatus Status { get; set; } = SagaStatus.NaoIniciada;
    public string CurrentStep { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int StepIndex { get; set; }
}
