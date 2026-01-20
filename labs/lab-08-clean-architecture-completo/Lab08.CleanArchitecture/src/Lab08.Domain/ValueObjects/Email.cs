using System.Text.RegularExpressions;
using Lab08.Domain.Exceptions;

namespace Lab08.Domain.ValueObjects;

/// <summary>
/// Value Object para Email com validação
/// </summary>
public sealed partial class Email : IEquatable<Email>
{
    private static readonly Regex EmailRegex = MyRegex();

    private Email(string valor)
    {
        Valor = valor;
    }

    public string Valor { get; }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email não pode ser vazio");

        var emailNormalizado = email.ToLowerInvariant().Trim();

        if (!EmailRegex.IsMatch(emailNormalizado))
            throw new DomainException("Formato de email inválido");

        return new Email(emailNormalizado);
    }

    public static bool TryParse(string email, out Email? result)
    {
        result = null;
        try
        {
            result = Create(email);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return EmailRegex.IsMatch(email.ToLowerInvariant().Trim());
    }

    // Implicit conversions
    public static implicit operator string(Email email) => email.Valor;
    public static explicit operator Email(string email) => Create(email);

    // Equality
    public bool Equals(Email? other) => other is not null && Valor == other.Valor;
    public override bool Equals(object? obj) => obj is Email other && Equals(other);
    public override int GetHashCode() => Valor.GetHashCode();
    public override string ToString() => Valor;

    public static bool operator ==(Email? left, Email? right) => Equals(left, right);
    public static bool operator !=(Email? left, Email? right) => !Equals(left, right);

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}
