namespace Lab10.Application.DTOs;

public record CategoriaDto(
    int Id,
    string Nome,
    string? Descricao,
    bool Ativo);

public record CategoriaCreateDto(
    string Nome,
    string? Descricao);

public record CategoriaUpdateDto(
    string Nome,
    string? Descricao);
