using Lab06.Domain.Exceptions;

namespace Lab06.Domain.ValueObjects;

/// <summary>
/// Value Object para Endereço completo
/// Domain layer - SEM dependências externas
/// </summary>
public sealed class Endereco : IEquatable<Endereco>
{
    public string Logradouro { get; private set; } = string.Empty;
    public string Numero { get; private set; } = string.Empty;
    public string? Complemento { get; private set; }
    public string Bairro { get; private set; } = string.Empty;
    public string Cidade { get; private set; } = string.Empty;
    public string Estado { get; private set; } = string.Empty;
    public string CEP { get; private set; } = string.Empty;

    // Construtor sem parâmetros para EF Core
#pragma warning disable CS8618
    private Endereco() { }
#pragma warning restore CS8618

    private Endereco(
        string logradouro,
        string numero,
        string? complemento,
        string bairro,
        string cidade,
        string estado,
        string cep)
    {
        Logradouro = logradouro;
        Numero = numero;
        Complemento = complemento;
        Bairro = bairro;
        Cidade = cidade;
        Estado = estado;
        CEP = cep;
    }

    public static Endereco Create(
        string logradouro,
        string numero,
        string? complemento,
        string bairro,
        string cidade,
        string estado,
        string cep)
    {
        if (string.IsNullOrWhiteSpace(logradouro))
            throw new DomainException("Logradouro é obrigatório");

        if (string.IsNullOrWhiteSpace(numero))
            throw new DomainException("Número é obrigatório");

        if (string.IsNullOrWhiteSpace(bairro))
            throw new DomainException("Bairro é obrigatório");

        if (string.IsNullOrWhiteSpace(cidade))
            throw new DomainException("Cidade é obrigatória");

        if (string.IsNullOrWhiteSpace(estado))
            throw new DomainException("Estado é obrigatório");

        if (string.IsNullOrWhiteSpace(cep))
            throw new DomainException("CEP é obrigatório");

        // Remove formatação do CEP
        var cepNumbers = new string(cep.Where(char.IsDigit).ToArray());
        if (cepNumbers.Length != 8)
            throw new DomainException("CEP deve ter 8 dígitos");

        if (estado.Length != 2)
            throw new DomainException("Estado deve ter 2 caracteres (UF)");

        return new Endereco(
            logradouro.Trim(),
            numero.Trim(),
            string.IsNullOrWhiteSpace(complemento) ? null : complemento.Trim(),
            bairro.Trim(),
            cidade.Trim(),
            estado.ToUpperInvariant().Trim(),
            cepNumbers
        );
    }

    /// <summary>
    /// CEP formatado: 00000-000
    /// </summary>
    public string CEPFormatado => CEP.Length == 8 ? $"{CEP[..5]}-{CEP[5..]}" : CEP;

    /// <summary>
    /// Endereço completo formatado
    /// </summary>
    public string EnderecoCompleto
    {
        get
        {
            var complementoPart = string.IsNullOrEmpty(Complemento) ? "" : $", {Complemento}";
            return $"{Logradouro}, {Numero}{complementoPart} - {Bairro}, {Cidade}/{Estado} - CEP: {CEPFormatado}";
        }
    }

    public bool Equals(Endereco? other)
    {
        if (other is null) return false;
        return Logradouro == other.Logradouro &&
               Numero == other.Numero &&
               Complemento == other.Complemento &&
               Bairro == other.Bairro &&
               Cidade == other.Cidade &&
               Estado == other.Estado &&
               CEP == other.CEP;
    }

    public override bool Equals(object? obj) => obj is Endereco other && Equals(other);

    public override int GetHashCode() => 
        HashCode.Combine(Logradouro, Numero, Complemento, Bairro, Cidade, Estado, CEP);

    public override string ToString() => EnderecoCompleto;

    public static bool operator ==(Endereco? left, Endereco? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(Endereco? left, Endereco? right) => !(left == right);
}
