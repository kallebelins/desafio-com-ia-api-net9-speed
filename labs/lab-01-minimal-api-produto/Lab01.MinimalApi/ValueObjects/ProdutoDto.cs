namespace Lab01.MinimalApi.ValueObjects;

public record ProdutoDto(
    int Id,
    string Nome,
    string Descricao,
    decimal Preco,
    bool Ativo,
    DateTime DataCriacao
);
