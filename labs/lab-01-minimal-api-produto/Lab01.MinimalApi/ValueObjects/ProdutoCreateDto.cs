namespace Lab01.MinimalApi.ValueObjects;

public record ProdutoCreateDto(
    string Nome,
    string Descricao,
    decimal Preco
);
