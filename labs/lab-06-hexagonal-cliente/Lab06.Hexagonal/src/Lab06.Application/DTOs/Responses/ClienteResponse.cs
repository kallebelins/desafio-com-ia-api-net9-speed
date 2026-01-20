namespace Lab06.Application.DTOs.Responses;

/// <summary>
/// Response com dados do cliente
/// </summary>
public record ClienteResponse(
    int Id,
    string Nome,
    string Email,
    string Cpf,
    string CpfFormatado,
    string? Telefone,
    bool Ativo,
    DateTime DataCriacao,
    DateTime? DataAtualizacao,
    EnderecoResponse? Endereco
);

/// <summary>
/// Response com dados do endereço
/// </summary>
public record EnderecoResponse(
    string Logradouro,
    string Numero,
    string? Complemento,
    string Bairro,
    string Cidade,
    string Estado,
    string CEP,
    string CEPFormatado,
    string EnderecoCompleto
);
