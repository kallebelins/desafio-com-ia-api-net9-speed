using Lab06.Domain.Exceptions;
using Lab06.Domain.ValueObjects;

namespace Lab06.Domain.Entities;

/// <summary>
/// Entidade de domínio Cliente
/// Domain layer - SEM dependências externas
/// </summary>
public class Cliente
{
    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public CPF Cpf { get; private set; } = null!;
    public Endereco? Endereco { get; private set; }
    public string? Telefone { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }

    // Construtor privado para EF Core
    private Cliente() { }

    public Cliente(string nome, string email, string cpf, string? telefone = null)
    {
        SetNome(nome);
        SetEmail(email);
        SetCpf(cpf);
        SetTelefone(telefone);
        Ativo = true;
        DataCriacao = DateTime.UtcNow;
    }

    public void SetNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("Nome não pode ser vazio");

        if (nome.Trim().Length < 3)
            throw new DomainException("Nome deve ter pelo menos 3 caracteres");

        if (nome.Trim().Length > 200)
            throw new DomainException("Nome deve ter no máximo 200 caracteres");

        Nome = nome.Trim();
        AtualizarDataModificacao();
    }

    public void SetEmail(string email)
    {
        Email = Email.Create(email);
        AtualizarDataModificacao();
    }

    public void SetCpf(string cpf)
    {
        Cpf = CPF.Create(cpf);
        AtualizarDataModificacao();
    }

    public void SetEndereco(Endereco endereco)
    {
        Endereco = endereco;
        AtualizarDataModificacao();
    }

    public void SetEndereco(
        string logradouro,
        string numero,
        string? complemento,
        string bairro,
        string cidade,
        string estado,
        string cep)
    {
        Endereco = Endereco.Create(logradouro, numero, complemento, bairro, cidade, estado, cep);
        AtualizarDataModificacao();
    }

    public void RemoverEndereco()
    {
        Endereco = null;
        AtualizarDataModificacao();
    }

    public void SetTelefone(string? telefone)
    {
        if (telefone != null)
        {
            var telefoneLimpo = new string(telefone.Where(char.IsDigit).ToArray());
            if (telefoneLimpo.Length < 10 || telefoneLimpo.Length > 11)
                throw new DomainException("Telefone deve ter 10 ou 11 dígitos");
            
            Telefone = telefoneLimpo;
        }
        else
        {
            Telefone = null;
        }
        AtualizarDataModificacao();
    }

    public void Ativar()
    {
        Ativo = true;
        AtualizarDataModificacao();
    }

    public void Desativar()
    {
        Ativo = false;
        AtualizarDataModificacao();
    }

    private void AtualizarDataModificacao()
    {
        if (Id > 0) // Só atualiza se já foi persistido
        {
            DataAtualizacao = DateTime.UtcNow;
        }
    }
}
