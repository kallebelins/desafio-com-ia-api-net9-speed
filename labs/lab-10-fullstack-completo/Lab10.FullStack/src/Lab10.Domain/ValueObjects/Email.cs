using System.Text.RegularExpressions;
using Lab10.Domain.Exceptions;

namespace Lab10.Domain.ValueObjects;

/// <summary>
/// Value Object Email com validação
/// </summary>
public sealed partial class Email : IEquatable<Email>
{
    private static readonly Regex EmailRegex = MyEmailRegex();

    private Email(string valor)
    {
        Valor = valor;
    }

    public string Valor { get; }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email não pode ser vazio");

        email = email.ToLowerInvariant().Trim();

        if (!IsValid(email))
            throw new DomainException("Email inválido");

        return new Email(email);
    }

    public static bool IsValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return EmailRegex.IsMatch(email);
    }

    public override bool Equals(object? obj) => obj is Email other && Equals(other);
    public bool Equals(Email? other) => other is not null && Valor == other.Valor;
    public override int GetHashCode() => Valor.GetHashCode();
    public override string ToString() => Valor;

    public static implicit operator string(Email email) => email.Valor;

    public static bool operator ==(Email? left, Email? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(Email? left, Email? right) => !(left == right);

    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled)]
    private static partial Regex MyEmailRegex();
}
