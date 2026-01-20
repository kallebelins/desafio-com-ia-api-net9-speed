using Lab08.Domain.Exceptions;

namespace Lab08.Domain.ValueObjects;

/// <summary>
/// Value Object para valores monetários
/// </summary>
public sealed class Money : IEquatable<Money>, IComparable<Money>
{
    private Money(decimal valor, string moeda)
    {
        Valor = Math.Round(valor, 2);
        Moeda = moeda.ToUpperInvariant();
    }

    public decimal Valor { get; }
    public string Moeda { get; }

    public static Money Create(decimal valor, string moeda = "BRL")
    {
        if (string.IsNullOrWhiteSpace(moeda))
            throw new DomainException("Moeda não pode ser vazia");

        if (moeda.Length != 3)
            throw new DomainException("Moeda deve ter 3 caracteres (ex: BRL, USD)");

        return new Money(valor, moeda);
    }

    public static Money Zero(string moeda = "BRL") => Create(0, moeda);

    // Operações aritméticas
    public static Money operator +(Money a, Money b)
    {
        ValidarMesmaMoeda(a, b);
        return Create(a.Valor + b.Valor, a.Moeda);
    }

    public static Money operator -(Money a, Money b)
    {
        ValidarMesmaMoeda(a, b);
        return Create(a.Valor - b.Valor, a.Moeda);
    }

    public static Money operator *(Money a, decimal multiplicador)
        => Create(a.Valor * multiplicador, a.Moeda);

    public static Money operator *(decimal multiplicador, Money a)
        => a * multiplicador;

    public static Money operator /(Money a, decimal divisor)
    {
        if (divisor == 0)
            throw new DomainException("Não é possível dividir por zero");
        return Create(a.Valor / divisor, a.Moeda);
    }

    // Comparações
    public static bool operator >(Money a, Money b)
    {
        ValidarMesmaMoeda(a, b);
        return a.Valor > b.Valor;
    }

    public static bool operator <(Money a, Money b)
    {
        ValidarMesmaMoeda(a, b);
        return a.Valor < b.Valor;
    }

    public static bool operator >=(Money a, Money b)
    {
        ValidarMesmaMoeda(a, b);
        return a.Valor >= b.Valor;
    }

    public static bool operator <=(Money a, Money b)
    {
        ValidarMesmaMoeda(a, b);
        return a.Valor <= b.Valor;
    }

    private static void ValidarMesmaMoeda(Money a, Money b)
    {
        if (a.Moeda != b.Moeda)
            throw new DomainException($"Não é possível operar valores de moedas diferentes: {a.Moeda} e {b.Moeda}");
    }

    // Equality
    public bool Equals(Money? other) => other is not null && Valor == other.Valor && Moeda == other.Moeda;
    public override bool Equals(object? obj) => obj is Money other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Valor, Moeda);

    public static bool operator ==(Money? left, Money? right) => Equals(left, right);
    public static bool operator !=(Money? left, Money? right) => !Equals(left, right);

    // IComparable
    public int CompareTo(Money? other)
    {
        if (other is null) return 1;
        ValidarMesmaMoeda(this, other);
        return Valor.CompareTo(other.Valor);
    }

    public override string ToString() => $"{Moeda} {Valor:N2}";
    public string ToStringFormatado() => Moeda == "BRL" 
        ? $"R$ {Valor:N2}" 
        : $"{Moeda} {Valor:N2}";
}
