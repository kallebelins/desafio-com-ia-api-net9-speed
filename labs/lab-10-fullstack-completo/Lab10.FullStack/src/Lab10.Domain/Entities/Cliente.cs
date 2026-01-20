using Mvp24Hours.Core.Entities;
using Lab10.Domain.ValueObjects;

namespace Lab10.Domain.Entities;

/// <summary>
/// Entidade Cliente do domínio de vendas
/// </summary>
public class Cliente : EntityBase<int>
{
    // Construtor protegido para EF Core
    protected Cliente() { }

    public Cliente(string nome, Email email, CPF cpf, Endereco? endereco = null)
    {
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Cpf = cpf ?? throw new ArgumentNullException(nameof(cpf));
        Endereco = endereco;
        Ativo = true;
        DataCadastro = DateTime.UtcNow;
    }

    public string Nome { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public CPF Cpf { get; private set; } = null!;
    public Endereco? Endereco { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataCadastro { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }

    // Navigation properties
    private readonly List<Venda> _vendas = new();
    public IReadOnlyCollection<Venda> Vendas => _vendas.AsReadOnly();

    public void Atualizar(string nome, Email email, Endereco? endereco)
    {
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Endereco = endereco;
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
