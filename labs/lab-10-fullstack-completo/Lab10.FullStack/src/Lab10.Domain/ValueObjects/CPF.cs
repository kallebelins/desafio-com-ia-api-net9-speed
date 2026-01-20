using Lab10.Domain.Exceptions;

namespace Lab10.Domain.ValueObjects;

/// <summary>
/// Value Object CPF com validação
/// </summary>
public sealed class CPF : IEquatable<CPF>
{
    private CPF(string valor)
    {
        Valor = valor;
    }

    public string Valor { get; }

    /// <summary>
    /// Retorna o CPF formatado (XXX.XXX.XXX-XX)
    /// </summary>
    public string Formatado => $"{Valor[..3]}.{Valor.Substring(3, 3)}.{Valor.Substring(6, 3)}-{Valor.Substring(9, 2)}";

    public static CPF Create(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new DomainException("CPF não pode ser vazio");

        // Remove caracteres não numéricos
        var cpfLimpo = new string(cpf.Where(char.IsDigit).ToArray());

        if (!IsValid(cpfLimpo))
            throw new DomainException("CPF inválido");

        return new CPF(cpfLimpo);
    }

    public static bool IsValid(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        // Remove caracteres não numéricos
        cpf = new string(cpf.Where(char.IsDigit).ToArray());

        if (cpf.Length != 11)
            return false;

        // Verifica se todos os dígitos são iguais
        if (cpf.Distinct().Count() == 1)
            return false;

        // Validação dos dígitos verificadores
        var tempCpf = cpf[..9];
        var soma = 0;

        for (var i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * (10 - i);

        var resto = soma % 11;
        var digito1 = resto < 2 ? 0 : 11 - resto;

        tempCpf += digito1;
        soma = 0;

        for (var i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * (11 - i);

        resto = soma % 11;
        var digito2 = resto < 2 ? 0 : 11 - resto;

        return cpf.EndsWith(digito1.ToString() + digito2.ToString());
    }

    public override bool Equals(object? obj) => obj is CPF other && Equals(other);
    public bool Equals(CPF? other) => other is not null && Valor == other.Valor;
    public override int GetHashCode() => Valor.GetHashCode();
    public override string ToString() => Formatado;

    public static implicit operator string(CPF cpf) => cpf.Valor;

    public static bool operator ==(CPF? left, CPF? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(CPF? left, CPF? right) => !(left == right);
}
