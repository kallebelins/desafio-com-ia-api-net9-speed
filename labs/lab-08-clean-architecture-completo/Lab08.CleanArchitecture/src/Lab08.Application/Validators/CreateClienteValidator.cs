using FluentValidation;
using Lab08.Application.UseCases.Clientes.CreateCliente;
using Lab08.Domain.ValueObjects;

namespace Lab08.Application.Validators;

/// <summary>
/// Validador para CreateClienteInput
/// </summary>
public class CreateClienteValidator : AbstractValidator<CreateClienteInput>
{
    public CreateClienteValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .MaximumLength(256).WithMessage("Email deve ter no máximo 256 caracteres")
            .Must(BeValidEmail).WithMessage("Email inválido");

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("CPF é obrigatório")
            .Must(BeValidCpf).WithMessage("CPF inválido");

        // Validações de endereço (quando fornecido)
        When(x => x.TemEndereco, () =>
        {
            RuleFor(x => x.Logradouro)
                .NotEmpty().WithMessage("Logradouro é obrigatório quando endereço é fornecido")
                .MaximumLength(200).WithMessage("Logradouro deve ter no máximo 200 caracteres");

            RuleFor(x => x.Numero)
                .NotEmpty().WithMessage("Número é obrigatório quando endereço é fornecido")
                .MaximumLength(20).WithMessage("Número deve ter no máximo 20 caracteres");

            RuleFor(x => x.Bairro)
                .NotEmpty().WithMessage("Bairro é obrigatório quando endereço é fornecido")
                .MaximumLength(100).WithMessage("Bairro deve ter no máximo 100 caracteres");

            RuleFor(x => x.Cidade)
                .NotEmpty().WithMessage("Cidade é obrigatória quando endereço é fornecido")
                .MaximumLength(100).WithMessage("Cidade deve ter no máximo 100 caracteres");

            RuleFor(x => x.Estado)
                .NotEmpty().WithMessage("Estado é obrigatório quando endereço é fornecido")
                .Length(2).WithMessage("Estado deve ter 2 caracteres (UF)");

            RuleFor(x => x.Cep)
                .NotEmpty().WithMessage("CEP é obrigatório quando endereço é fornecido")
                .Matches(@"^\d{5}-?\d{3}$|^\d{8}$").WithMessage("CEP inválido");
        });
    }

    private static bool BeValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;
        return Email.IsValid(email);
    }

    private static bool BeValidCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;
        return Cpf.IsValid(cpf);
    }
}
