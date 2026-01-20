using Mvp24Hours.Core.Entities;

namespace Lab04.Core.Entities;

/// <summary>
/// Entidade Cliente com campos de auditoria
/// </summary>
public class Cliente : EntityBase<int>
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataAtualizacao { get; set; }
}
