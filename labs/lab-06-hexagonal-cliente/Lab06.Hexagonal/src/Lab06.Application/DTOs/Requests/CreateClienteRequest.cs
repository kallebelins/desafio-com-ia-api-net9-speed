namespace Lab06.Application.DTOs.Requests;

/// <summary>
/// Request para criar um novo cliente
/// </summary>
public record CreateClienteRequest(
    string Nome,
    string Email,
    string Cpf,
    string? Telefone = null,
    EnderecoRequest? Endereco = null
);

/// <summary>
/// Request para endereço
/// </summary>
public record EnderecoRequest(
    string Logradouro,
    string Numero,
    string? Complemento,
    string Bairro,
    string Cidade,
    string Estado,
    string CEP
);
