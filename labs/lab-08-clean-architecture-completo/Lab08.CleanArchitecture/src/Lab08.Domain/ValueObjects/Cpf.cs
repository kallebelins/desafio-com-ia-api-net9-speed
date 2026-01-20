using System.Text.RegularExpressions;
using Lab08.Domain.Exceptions;

namespace Lab08.Domain.ValueObjects;

/// <summary>
/// Value Object para CPF brasileiro com validação
/// </summary>
public sealed partial class Cpf : IEquatable<Cpf>
{
    private static readonly Regex CpfRegex = MyRegex();

    private Cpf(string valor)
    {
        Valor = valor;
    }

    public string Valor { get; }
    public string Formatado => $"{Valor[..3]}.{Valor.Substring(3, 3)}.{Valor.Substring(6, 3)}-{Valor[9..]}";

    public static Cpf Create(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new DomainException("CPF não pode ser vazio");

        var cpfLimpo = CpfRegex.Replace(cpf, "");

        if (cpfLimpo.Length != 11)
            throw new DomainException("CPF deve conter 11 dígitos");

        if (!ValidarDigitosVerificadores(cpfLimpo))
            throw new DomainException("CPF inválido");

        return new Cpf(cpfLimpo);
    }

    public static bool TryParse(string cpf, out Cpf? result)
    {
        result = null;
        try
        {
            result = Create(cpf);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsValid(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        var cpfLimpo = CpfRegex.Replace(cpf, "");
        
        if (cpfLimpo.Length != 11)
            return false;

        return ValidarDigitosVerificadores(cpfLimpo);
    }

    private static bool ValidarDigitosVerificadores(string cpf)
    {
        // Verifica se todos os dígitos são iguais
        if (cpf.Distinct().Count() == 1)
            return false;

        // Calcula primeiro dígito verificador
        var soma = 0;
        for (var i = 0; i < 9; i++)
            soma += int.Parse(cpf[i].ToString()) * (10 - i);

        var resto = soma % 11;
        var digito1 = resto < 2 ? 0 : 11 - resto;

        if (int.Parse(cpf[9].ToString()) != digito1)
            return false;

        // Calcula segundo dígito verificador
        soma = 0;
        for (var i = 0; i < 10; i++)
            soma += int.Parse(cpf[i].ToString()) * (11 - i);

        resto = soma % 11;
        var digito2 = resto < 2 ? 0 : 11 - resto;

        return int.Parse(cpf[10].ToString()) == digito2;
    }

    // Implicit conversions
    public static implicit operator string(Cpf cpf) => cpf.Valor;
    public static explicit operator Cpf(string cpf) => Create(cpf);

    // Equality
    public bool Equals(Cpf? other) => other is not null && Valor == other.Valor;
    public override bool Equals(object? obj) => obj is Cpf other && Equals(other);
    public override int GetHashCode() => Valor.GetHashCode();
    public override string ToString() => Formatado;

    public static bool operator ==(Cpf? left, Cpf? right) => Equals(left, right);
    public static bool operator !=(Cpf? left, Cpf? right) => !Equals(left, right);

    [GeneratedRegex(@"[^\d]")]
    private static partial Regex MyRegex();
}
