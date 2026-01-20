namespace Lab02.Core.ValueObjects;

public record ClienteUpdateDto(
    string Nome,
    string Email,
    string Telefone,
    bool Ativo
);
