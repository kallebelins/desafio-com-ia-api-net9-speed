namespace Lab02.Core.ValueObjects;

public record ClienteDto(
    int Id,
    string Nome,
    string Email,
    string Telefone,
    bool Ativo,
    DateTime DataCriacao
);
