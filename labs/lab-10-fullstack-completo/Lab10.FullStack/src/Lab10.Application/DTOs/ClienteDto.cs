namespace Lab10.Application.DTOs;

public record ClienteDto(
    int Id,
    string Nome,
    string Email,
    string Cpf,
    EnderecoDto? Endereco,
    bool Ativo,
    DateTime DataCadastro);

public record ClienteCreateDto(
    string Nome,
    string Email,
    string Cpf,
    EnderecoDto? Endereco);

public record ClienteUpdateDto(
    string Nome,
    string Email,
    EnderecoDto? Endereco);

public record EnderecoDto(
    string Logradouro,
    string Numero,
    string? Complemento,
    string Bairro,
    string Cidade,
    string Estado,
    string Cep);
