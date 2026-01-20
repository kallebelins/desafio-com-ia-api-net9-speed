namespace Lab08.Application.UseCases.Clientes.CreateCliente;

/// <summary>
/// Input para criação de cliente
/// </summary>
public record CreateClienteInput
{
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Cpf { get; init; } = string.Empty;
    
    // Endereço opcional
    public string? Logradouro { get; init; }
    public string? Numero { get; init; }
    public string? Complemento { get; init; }
    public string? Bairro { get; init; }
    public string? Cidade { get; init; }
    public string? Estado { get; init; }
    public string? Cep { get; init; }

    public bool TemEndereco => !string.IsNullOrWhiteSpace(Logradouro);
}
