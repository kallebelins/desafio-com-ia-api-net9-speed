using Mvp24Hours.Core.Entities;
using Lab10.Domain.ValueObjects;
using Lab10.Domain.Exceptions;

namespace Lab10.Domain.Entities;

/// <summary>
/// Entidade Produto do domínio de vendas
/// </summary>
public class Produto : EntityBase<int>
{
    // Construtor protegido para EF Core
    protected Produto() { }

    public Produto(string nome, string? descricao, Money precoUnitario, int estoqueAtual, int categoriaId)
    {
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        Descricao = descricao;
        PrecoUnitario = precoUnitario ?? throw new ArgumentNullException(nameof(precoUnitario));
        EstoqueAtual = estoqueAtual >= 0 ? estoqueAtual : throw new DomainException("Estoque não pode ser negativo");
        EstoqueReservado = 0;
        CategoriaId = categoriaId;
        Ativo = true;
        DataCadastro = DateTime.UtcNow;
    }

    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public Money PrecoUnitario { get; private set; } = null!;
    public int EstoqueAtual { get; private set; }
    public int EstoqueReservado { get; private set; }
    public int CategoriaId { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataCadastro { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }

    // Navigation property
    public Categoria? Categoria { get; private set; }

    /// <summary>
    /// Estoque disponível = Estoque Atual - Estoque Reservado
    /// </summary>
    public int EstoqueDisponivel => EstoqueAtual - EstoqueReservado;

    public void Atualizar(string nome, string? descricao, Money precoUnitario, int categoriaId)
    {
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        Descricao = descricao;
        PrecoUnitario = precoUnitario ?? throw new ArgumentNullException(nameof(precoUnitario));
        CategoriaId = categoriaId;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AdicionarEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new DomainException("Quantidade deve ser maior que zero");

        EstoqueAtual += quantidade;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void ReservarEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new DomainException("Quantidade deve ser maior que zero");

        if (quantidade > EstoqueDisponivel)
            throw new DomainException($"Estoque insuficiente. Disponível: {EstoqueDisponivel}");

        EstoqueReservado += quantidade;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void LiberarReserva(int quantidade)
    {
        if (quantidade <= 0)
            throw new DomainException("Quantidade deve ser maior que zero");

        if (quantidade > EstoqueReservado)
            throw new DomainException("Quantidade a liberar é maior que a reservada");

        EstoqueReservado -= quantidade;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void BaixarEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new DomainException("Quantidade deve ser maior que zero");

        if (quantidade > EstoqueReservado)
            throw new DomainException("Quantidade a baixar é maior que a reservada");

        EstoqueAtual -= quantidade;
        EstoqueReservado -= quantidade;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Ativar()
    {
        Ativo = true;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Desativar()
    {
        Ativo = false;
        DataAtualizacao = DateTime.UtcNow;
    }
}
