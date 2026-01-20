using Mvp24Hours.Core.Entities;

namespace Lab10.Domain.Entities;

/// <summary>
/// Entidade Categoria para classificar produtos
/// </summary>
public class Categoria : EntityBase<int>
{
    // Construtor protegido para EF Core
    protected Categoria() { }

    public Categoria(string nome, string? descricao = null)
    {
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        Descricao = descricao;
        Ativo = true;
        DataCadastro = DateTime.UtcNow;
    }

    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataCadastro { get; private set; }

    // Navigation properties
    private readonly List<Produto> _produtos = new();
    public IReadOnlyCollection<Produto> Produtos => _produtos.AsReadOnly();

    public void Atualizar(string nome, string? descricao)
    {
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        Descricao = descricao;
    }

    public void Ativar() => Ativo = true;
    public void Desativar() => Ativo = false;
}
