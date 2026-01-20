using Lab08.Domain.Exceptions;

namespace Lab08.Domain.ValueObjects;

/// <summary>
/// Value Object para Endereço
/// </summary>
public sealed class Endereco : IEquatable<Endereco>
{
    private Endereco(string logradouro, string numero, string? complemento, 
        string bairro, string cidade, string estado, string cep)
    {
        Logradouro = logradouro;
        Numero = numero;
        Complemento = complemento;
        Bairro = bairro;
        Cidade = cidade;
        Estado = estado;
        Cep = cep;
    }

    public string Logradouro { get; }
    public string Numero { get; }
    public string? Complemento { get; }
    public string Bairro { get; }
    public string Cidade { get; }
    public string Estado { get; }
    public string Cep { get; }

    public string EnderecoCompleto => string.IsNullOrEmpty(Complemento)
        ? $"{Logradouro}, {Numero} - {Bairro}, {Cidade}/{Estado} - CEP: {CepFormatado}"
        : $"{Logradouro}, {Numero}, {Complemento} - {Bairro}, {Cidade}/{Estado} - CEP: {CepFormatado}";

    public string CepFormatado => $"{Cep[..5]}-{Cep[5..]}";

    public static Endereco Create(string logradouro, string numero, string? complemento,
        string bairro, string cidade, string estado, string cep)
    {
        if (string.IsNullOrWhiteSpace(logradouro))
            throw new DomainException("Logradouro não pode ser vazio");

        if (string.IsNullOrWhiteSpace(numero))
            throw new DomainException("Número não pode ser vazio");

        if (string.IsNullOrWhiteSpace(bairro))
            throw new DomainException("Bairro não pode ser vazio");

        if (string.IsNullOrWhiteSpace(cidade))
            throw new DomainException("Cidade não pode ser vazia");

        if (string.IsNullOrWhiteSpace(estado))
            throw new DomainException("Estado não pode ser vazio");

        if (estado.Length != 2)
            throw new DomainException("Estado deve ter 2 caracteres (UF)");

        var cepLimpo = new string(cep?.Where(char.IsDigit).ToArray() ?? []);
        if (cepLimpo.Length != 8)
            throw new DomainException("CEP deve ter 8 dígitos");

        return new Endereco(
            logradouro.Trim(),
            numero.Trim(),
            complemento?.Trim(),
            bairro.Trim(),
            cidade.Trim(),
            estado.ToUpperInvariant().Trim(),
            cepLimpo
        );
    }

    // Equality
    public bool Equals(Endereco? other)
    {
        if (other is null) return false;
        return Logradouro == other.Logradouro &&
               Numero == other.Numero &&
               Complemento == other.Complemento &&
               Bairro == other.Bairro &&
               Cidade == other.Cidade &&
               Estado == other.Estado &&
               Cep == other.Cep;
    }

    public override bool Equals(object? obj) => obj is Endereco other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Logradouro, Numero, Bairro, Cidade, Estado, Cep);
    public override string ToString() => EnderecoCompleto;

    public static bool operator ==(Endereco? left, Endereco? right) => Equals(left, right);
    public static bool operator !=(Endereco? left, Endereco? right) => !Equals(left, right);
}
