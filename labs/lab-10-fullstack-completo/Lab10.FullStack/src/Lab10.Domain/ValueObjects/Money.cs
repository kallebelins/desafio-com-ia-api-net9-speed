using Lab10.Domain.Exceptions;

namespace Lab10.Domain.ValueObjects;

/// <summary>
/// Value Object Money para representar valores monetários
/// </summary>
public sealed class Money : IEquatable<Money>, IComparable<Money>
{
    public const string MoedaPadrao = "BRL";

    private Money(decimal valor, string moeda = MoedaPadrao)
    {
        Valor = Math.Round(valor, 2);
        Moeda = moeda;
    }

    public decimal Valor { get; }
    public string Moeda { get; }

    public static Money Create(decimal valor, string moeda = MoedaPadrao)
    {
        if (valor < 0)
            throw new DomainException("Valor monetário não pode ser negativo");

        return new Money(valor, moeda);
    }

    public static Money Zero(string moeda = MoedaPadrao) => new(0, moeda);

    public Money Add(Money other)
    {
        ValidarMesmaMoeda(other);
        return new Money(Valor + other.Valor, Moeda);
    }

    public Money Subtract(Money other)
    {
        ValidarMesmaMoeda(other);
        var resultado = Valor - other.Valor;
        if (resultado < 0)
            throw new DomainException("Resultado não pode ser negativo");

        return new Money(resultado, Moeda);
    }

    public Money Multiply(decimal fator)
    {
        if (fator < 0)
            throw new DomainException("Fator de multiplicação não pode ser negativo");

        return new Money(Valor * fator, Moeda);
    }

    private void ValidarMesmaMoeda(Money other)
    {
        if (Moeda != other.Moeda)
            throw new DomainException($"Não é possível operar moedas diferentes: {Moeda} e {other.Moeda}");
    }

    public string Formatado => $"{Moeda} {Valor:N2}";

    public override bool Equals(object? obj) => obj is Money other && Equals(other);
    public bool Equals(Money? other) => other is not null && Valor == other.Valor && Moeda == other.Moeda;
    public override int GetHashCode() => HashCode.Combine(Valor, Moeda);
    public override string ToString() => Formatado;

    public int CompareTo(Money? other)
    {
        if (other is null) return 1;
        ValidarMesmaMoeda(other);
        return Valor.CompareTo(other.Valor);
    }

    public static implicit operator decimal(Money money) => money.Valor;

    public static Money operator +(Money left, Money right) => left.Add(right);
    public static Money operator -(Money left, Money right) => left.Subtract(right);
    public static Money operator *(Money left, decimal right) => left.Multiply(right);

    public static bool operator ==(Money? left, Money? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(Money? left, Money? right) => !(left == right);
    public static bool operator <(Money left, Money right) => left.CompareTo(right) < 0;
    public static bool operator >(Money left, Money right) => left.CompareTo(right) > 0;
    public static bool operator <=(Money left, Money right) => left.CompareTo(right) <= 0;
    public static bool operator >=(Money left, Money right) => left.CompareTo(right) >= 0;
}
