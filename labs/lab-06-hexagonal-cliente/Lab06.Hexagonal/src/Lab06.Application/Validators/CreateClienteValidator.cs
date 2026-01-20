using FluentValidation;
using Lab06.Application.DTOs.Requests;
using Lab06.Domain.ValueObjects;

namespace Lab06.Application.Validators;

/// <summary>
/// Validador para CreateClienteRequest usando FluentValidation
/// </summary>
public class CreateClienteValidator : AbstractValidator<CreateClienteRequest>
{
    public CreateClienteValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MinimumLength(3).WithMessage("Nome deve ter pelo menos 3 caracteres")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .Must(BeValidEmail).WithMessage("Email inválido");

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("CPF é obrigatório")
            .Must(BeValidCpf).WithMessage("CPF inválido");

        RuleFor(x => x.Telefone)
            .Must(BeValidTelefone).WithMessage("Telefone deve ter 10 ou 11 dígitos")
            .When(x => !string.IsNullOrEmpty(x.Telefone));

        When(x => x.Endereco != null, () =>
        {
            RuleFor(x => x.Endereco!.Logradouro)
                .NotEmpty().WithMessage("Logradouro é obrigatório");

            RuleFor(x => x.Endereco!.Numero)
                .NotEmpty().WithMessage("Número é obrigatório");

            RuleFor(x => x.Endereco!.Bairro)
                .NotEmpty().WithMessage("Bairro é obrigatório");

            RuleFor(x => x.Endereco!.Cidade)
                .NotEmpty().WithMessage("Cidade é obrigatória");

            RuleFor(x => x.Endereco!.Estado)
                .NotEmpty().WithMessage("Estado é obrigatório")
                .Length(2).WithMessage("Estado deve ter 2 caracteres (UF)");

            RuleFor(x => x.Endereco!.CEP)
                .NotEmpty().WithMessage("CEP é obrigatório")
                .Must(BeValidCep).WithMessage("CEP deve ter 8 dígitos");
        });
    }

    private static bool BeValidEmail(string email)
    {
        return Email.IsValid(email);
    }

    private static bool BeValidCpf(string cpf)
    {
        return CPF.IsValid(cpf);
    }

    private static bool BeValidTelefone(string? telefone)
    {
        if (string.IsNullOrEmpty(telefone))
            return true;

        var telefoneLimpo = new string(telefone.Where(char.IsDigit).ToArray());
        return telefoneLimpo.Length >= 10 && telefoneLimpo.Length <= 11;
    }

    private static bool BeValidCep(string cep)
    {
        var cepLimpo = new string(cep.Where(char.IsDigit).ToArray());
        return cepLimpo.Length == 8;
    }
}
