using System.Text.RegularExpressions;
using Lab06.Domain.Exceptions;

namespace Lab06.Domain.ValueObjects;

/// <summary>
/// Value Object para CPF brasileiro com validação completa
/// Domain layer - SEM dependências externas
/// </summary>
public sealed partial class CPF : IEquatable<CPF>
{
    private static readonly Regex CpfRegex = GenerateCpfRegex();
    
    /// <summary>
    /// CPF sem formatação (apenas números)
    /// </summary>
    public string Value { get; }
    
    /// <summary>
    /// CPF formatado: 000.000.000-00
    /// </summary>
    public string Formatted => FormatCpf(Value);
    
    /// <summary>
    /// CPF sem formatação (alias para Value)
    /// </summary>
    public string Unformatted => Value;

    private CPF(string value)
    {
        Value = value;
    }

    public static CPF Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("CPF não pode ser vazio");

        var cpfNumbers = ExtractNumbers(value);

        if (cpfNumbers.Length != 11)
            throw new DomainException($"CPF deve ter 11 dígitos: {value}");

        if (!ValidateCpfAlgorithm(cpfNumbers))
            throw new DomainException($"CPF inválido: {value}");

        return new CPF(cpfNumbers);
    }

    public static bool TryParse(string value, out CPF? result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        var cpfNumbers = ExtractNumbers(value);

        if (cpfNumbers.Length != 11)
            return false;

        if (!ValidateCpfAlgorithm(cpfNumbers))
            return false;

        result = new CPF(cpfNumbers);
        return true;
    }

    public static bool IsValid(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var cpfNumbers = ExtractNumbers(value);

        if (cpfNumbers.Length != 11)
            return false;

        return ValidateCpfAlgorithm(cpfNumbers);
    }

    private static string ExtractNumbers(string value)
    {
        return CpfRegex.Replace(value, "");
    }

    private static bool ValidateCpfAlgorithm(string cpf)
    {
        // Verifica se todos os dígitos são iguais (CPF inválido)
        if (cpf.Distinct().Count() == 1)
            return false;

        // Calcula o primeiro dígito verificador
        int sum = 0;
        for (int i = 0; i < 9; i++)
            sum += (cpf[i] - '0') * (10 - i);

        int firstDigit = (sum * 10) % 11;
        if (firstDigit == 10) firstDigit = 0;

        if (cpf[9] - '0' != firstDigit)
            return false;

        // Calcula o segundo dígito verificador
        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += (cpf[i] - '0') * (11 - i);

        int secondDigit = (sum * 10) % 11;
        if (secondDigit == 10) secondDigit = 0;

        return cpf[10] - '0' == secondDigit;
    }

    private static string FormatCpf(string cpf)
    {
        return $"{cpf[..3]}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
    }

    public bool Equals(CPF? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => obj is CPF other && Equals(other);
    
    public override int GetHashCode() => Value.GetHashCode();
    
    public override string ToString() => Formatted;

    public static implicit operator string(CPF cpf) => cpf.Value;
    
    public static explicit operator CPF(string value) => Create(value);

    public static bool operator ==(CPF? left, CPF? right) => 
        left is null ? right is null : left.Equals(right);
    
    public static bool operator !=(CPF? left, CPF? right) => !(left == right);

    [GeneratedRegex(@"\D", RegexOptions.Compiled)]
    private static partial Regex GenerateCpfRegex();
}
