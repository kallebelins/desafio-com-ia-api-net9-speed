namespace Lab10.Domain.ValueObjects;

/// <summary>
/// Value Object Endereco
/// </summary>
public sealed class Endereco : IEquatable<Endereco>
{
    private Endereco(string logradouro, string numero, string? complemento, string bairro, string cidade, string estado, string cep)
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

    public static Endereco Create(
        string logradouro,
        string numero,
        string? complemento,
        string bairro,
        string cidade,
        string estado,
        string cep)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(logradouro);
        ArgumentException.ThrowIfNullOrWhiteSpace(numero);
        ArgumentException.ThrowIfNullOrWhiteSpace(bairro);
        ArgumentException.ThrowIfNullOrWhiteSpace(cidade);
        ArgumentException.ThrowIfNullOrWhiteSpace(estado);
        ArgumentException.ThrowIfNullOrWhiteSpace(cep);

        // Remove caracteres não numéricos do CEP
        var cepLimpo = new string(cep.Where(char.IsDigit).ToArray());
        if (cepLimpo.Length != 8)
            throw new ArgumentException("CEP deve ter 8 dígitos");

        return new Endereco(
            logradouro.Trim(),
            numero.Trim(),
            complemento?.Trim(),
            bairro.Trim(),
            cidade.Trim(),
            estado.Trim().ToUpperInvariant(),
            cepLimpo);
    }

    public string CepFormatado => $"{Cep[..5]}-{Cep.Substring(5, 3)}";

    public string Completo
    {
        get
        {
            var endereco = $"{Logradouro}, {Numero}";
            if (!string.IsNullOrWhiteSpace(Complemento))
                endereco += $" - {Complemento}";
            endereco += $", {Bairro}, {Cidade}/{Estado} - {CepFormatado}";
            return endereco;
        }
    }

    public override bool Equals(object? obj) => obj is Endereco other && Equals(other);

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

    public override int GetHashCode() =>
        HashCode.Combine(Logradouro, Numero, Complemento, Bairro, Cidade, Estado, Cep);

    public override string ToString() => Completo;

    public static bool operator ==(Endereco? left, Endereco? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(Endereco? left, Endereco? right) => !(left == right);
}
