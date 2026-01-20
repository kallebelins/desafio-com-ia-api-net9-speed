namespace Lab07.Core.ValueObjects;

/// <summary>
/// DTO para transferência de dados de Cliente
/// </summary>
public record ClienteDto
{
    public Guid Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Telefone { get; init; } = string.Empty;
    public bool Ativo { get; init; }
    public DateTime Created { get; init; }
}
