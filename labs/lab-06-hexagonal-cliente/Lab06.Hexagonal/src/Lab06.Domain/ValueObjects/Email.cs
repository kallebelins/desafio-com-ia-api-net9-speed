using System.Text.RegularExpressions;
using Lab06.Domain.Exceptions;

namespace Lab06.Domain.ValueObjects;

/// <summary>
/// Value Object para Email com validação
/// Domain layer - SEM dependências externas
/// </summary>
public sealed partial class Email : IEquatable<Email>
{
    private static readonly Regex EmailRegex = GenerateEmailRegex();
    
    public string Value { get; }
    public string LocalPart => Value.Split('@')[0];
    public string Domain => Value.Split('@')[1];

    private Email(string value)
    {
        Value = value.ToLowerInvariant().Trim();
    }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email não pode ser vazio");

        value = value.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(value))
            throw new DomainException($"Email inválido: {value}");

        return new Email(value);
    }

    public static bool TryParse(string value, out Email? result)
    {
        result = null;
        
        if (string.IsNullOrWhiteSpace(value))
            return false;

        value = value.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(value))
            return false;

        result = new Email(value);
        return true;
    }

    public static bool IsValid(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;
        return EmailRegex.IsMatch(value.Trim());
    }

    public bool Equals(Email? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => obj is Email other && Equals(other);
    
    public override int GetHashCode() => Value.GetHashCode();
    
    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
    
    public static explicit operator Email(string value) => Create(value);

    public static bool operator ==(Email? left, Email? right) => 
        left is null ? right is null : left.Equals(right);
    
    public static bool operator !=(Email? left, Email? right) => !(left == right);

    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled)]
    private static partial Regex GenerateEmailRegex();
}
