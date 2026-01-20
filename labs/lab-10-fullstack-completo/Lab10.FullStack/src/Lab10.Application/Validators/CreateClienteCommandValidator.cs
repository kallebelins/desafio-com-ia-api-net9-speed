using FluentValidation;
using Lab10.Application.Commands.Clientes;
using Lab10.Domain.ValueObjects;

namespace Lab10.Application.Validators;

public class CreateClienteCommandValidator : AbstractValidator<CreateClienteCommand>
{
    public CreateClienteCommandValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .Must(email => Email.IsValid(email)).WithMessage("Email inválido");

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("CPF é obrigatório")
            .Must(cpf => CPF.IsValid(cpf)).WithMessage("CPF inválido");

        When(x => x.Endereco != null, () =>
        {
            RuleFor(x => x.Endereco!.Logradouro)
                .NotEmpty().WithMessage("Logradouro é obrigatório")
                .MaximumLength(200).WithMessage("Logradouro deve ter no máximo 200 caracteres");

            RuleFor(x => x.Endereco!.Numero)
                .NotEmpty().WithMessage("Número é obrigatório");

            RuleFor(x => x.Endereco!.Bairro)
                .NotEmpty().WithMessage("Bairro é obrigatório");

            RuleFor(x => x.Endereco!.Cidade)
                .NotEmpty().WithMessage("Cidade é obrigatória");

            RuleFor(x => x.Endereco!.Estado)
                .NotEmpty().WithMessage("Estado é obrigatório")
                .Length(2).WithMessage("Estado deve ter 2 caracteres");

            RuleFor(x => x.Endereco!.Cep)
                .NotEmpty().WithMessage("CEP é obrigatório")
                .Matches(@"^\d{5}-?\d{3}$").WithMessage("CEP inválido");
        });
    }
}
