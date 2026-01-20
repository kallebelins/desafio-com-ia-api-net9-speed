namespace Lab02.Core.ValueObjects;

public record ClienteCreateDto(
    string Nome,
    string Email,
    string Telefone
);
