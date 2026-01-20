namespace Lab01.MinimalApi.ValueObjects;

public record ProdutoUpdateDto(
    string Nome,
    string Descricao,
    decimal Preco,
    bool Ativo
);
