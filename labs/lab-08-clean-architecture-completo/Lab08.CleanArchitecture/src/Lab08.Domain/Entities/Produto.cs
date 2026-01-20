using Lab08.Domain.Exceptions;
using Lab08.Domain.ValueObjects;
using Mvp24Hours.Core.Entities;

namespace Lab08.Domain.Entities;

/// <summary>
/// Entidade Produto
/// </summary>
public class Produto : EntityBase<int>
{
    // Construtor para EF Core
    protected Produto() { }

    public Produto(string nome, string? descricao, Money preco, int estoque, int categoriaId)
    {
        Nome = nome;
        Descricao = descricao;
        Preco = preco;
        Estoque = estoque;
        CategoriaId = categoriaId;
        Ativo = true;
        DataCadastro = DateTime.UtcNow;
    }

    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public Money Preco { get; private set; } = null!;
    public int Estoque { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataCadastro { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }

    // Relacionamentos
    public int CategoriaId { get; private set; }
    public virtual Categoria Categoria { get; private set; } = null!;
    public virtual ICollection<ItemVenda> ItensVenda { get; private set; } = new List<ItemVenda>();

    // Métodos de domínio
    public void AtualizarDados(string nome, string? descricao, Money preco, int categoriaId)
    {
        Nome = nome;
        Descricao = descricao;
        Preco = preco;
        CategoriaId = categoriaId;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AdicionarEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new DomainException("Quantidade deve ser maior que zero");

        Estoque += quantidade;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void RemoverEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new DomainException("Quantidade deve ser maior que zero");

        if (Estoque < quantidade)
            throw new DomainException($"Estoque insuficiente. Disponível: {Estoque}, Solicitado: {quantidade}");

        Estoque -= quantidade;
        DataAtualizacao = DateTime.UtcNow;
    }

    public bool TemEstoqueDisponivel(int quantidade) => Estoque >= quantidade;

    public void Desativar()
    {
        Ativo = false;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Ativar()
    {
        Ativo = true;
        DataAtualizacao = DateTime.UtcNow;
    }
}
