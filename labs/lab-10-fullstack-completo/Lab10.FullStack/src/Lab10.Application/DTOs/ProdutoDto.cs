namespace Lab10.Application.DTOs;

public record ProdutoDto(
    int Id,
    string Nome,
    string? Descricao,
    decimal PrecoUnitario,
    int EstoqueAtual,
    int EstoqueReservado,
    int EstoqueDisponivel,
    int CategoriaId,
    string? CategoriaNome,
    bool Ativo);

public record ProdutoCreateDto(
    string Nome,
    string? Descricao,
    decimal PrecoUnitario,
    int EstoqueInicial,
    int CategoriaId);

public record ProdutoUpdateDto(
    string Nome,
    string? Descricao,
    decimal PrecoUnitario,
    int CategoriaId);

public record AtualizarEstoqueDto(
    int Quantidade);
