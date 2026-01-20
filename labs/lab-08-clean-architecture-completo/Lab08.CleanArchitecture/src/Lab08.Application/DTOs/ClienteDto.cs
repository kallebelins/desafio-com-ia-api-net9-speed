namespace Lab08.Application.DTOs;

/// <summary>
/// DTO para Cliente
/// </summary>
public record ClienteDto
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Cpf { get; init; } = string.Empty;
    public string? CpfFormatado { get; init; }
    public EnderecoDto? Endereco { get; init; }
    public bool Ativo { get; init; }
    public DateTime DataCadastro { get; init; }
    public DateTime? DataAtualizacao { get; init; }
}

/// <summary>
/// DTO resumido para Cliente (listagens)
/// </summary>
public record ClienteResumoDto
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool Ativo { get; init; }
}

/// <summary>
/// DTO para Endereço
/// </summary>
public record EnderecoDto
{
    public string Logradouro { get; init; } = string.Empty;
    public string Numero { get; init; } = string.Empty;
    public string? Complemento { get; init; }
    public string Bairro { get; init; } = string.Empty;
    public string Cidade { get; init; } = string.Empty;
    public string Estado { get; init; } = string.Empty;
    public string Cep { get; init; } = string.Empty;
    public string? EnderecoCompleto { get; init; }
}
