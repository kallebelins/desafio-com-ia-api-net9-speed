using Mvp24Hours.Core.Entities;

namespace Lab08.Domain.Entities;

/// <summary>
/// Entidade Categoria de Produtos
/// </summary>
public class Categoria : EntityBase<int>
{
    // Construtor para EF Core
    protected Categoria() { }

    public Categoria(string nome, string? descricao = null)
    {
        Nome = nome;
        Descricao = descricao;
        Ativo = true;
    }

    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public bool Ativo { get; private set; }

    // Relacionamentos
    public virtual ICollection<Produto> Produtos { get; private set; } = new List<Produto>();

    // Métodos de domínio
    public void AtualizarDados(string nome, string? descricao)
    {
        Nome = nome;
        Descricao = descricao;
    }

    public void Desativar() => Ativo = false;
    public void Ativar() => Ativo = true;
}
