using Lab08.Domain.ValueObjects;
using Mvp24Hours.Core.Entities;

namespace Lab08.Domain.Entities;

/// <summary>
/// Entidade Cliente - Aggregate Root
/// </summary>
public class Cliente : EntityBase<int>
{
    // Construtor para EF Core
    protected Cliente() { }

    public Cliente(string nome, Email email, Cpf cpf, Endereco? endereco = null)
    {
        Nome = nome;
        Email = email;
        Cpf = cpf;
        Endereco = endereco;
        Ativo = true;
        DataCadastro = DateTime.UtcNow;
    }

    public string Nome { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public Cpf Cpf { get; private set; } = null!;
    public Endereco? Endereco { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataCadastro { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }

    // Relacionamentos
    public virtual ICollection<Venda> Vendas { get; private set; } = new List<Venda>();

    // Métodos de domínio
    public void AtualizarDados(string nome, Email email, Endereco? endereco)
    {
        Nome = nome;
        Email = email;
        Endereco = endereco;
        DataAtualizacao = DateTime.UtcNow;
    }

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
