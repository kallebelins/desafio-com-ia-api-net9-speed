namespace Lab06.Application.DTOs.Requests;

/// <summary>
/// Request para atualizar um cliente existente
/// </summary>
public record UpdateClienteRequest(
    string Nome,
    string Email,
    string? Telefone = null,
    bool? Ativo = null,
    EnderecoRequest? Endereco = null
);
