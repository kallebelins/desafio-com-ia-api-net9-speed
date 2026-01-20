using FluentValidation;
using Lab02.Core.ValueObjects;

namespace Lab02.Core.Validators;

public class ClienteCreateValidator : AbstractValidator<ClienteCreateDto>
{
    public ClienteCreateValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(100).WithMessage("Nome não pode exceder 100 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido")
            .MaximumLength(255).WithMessage("Email não pode exceder 255 caracteres");

        RuleFor(x => x.Telefone)
            .NotEmpty().WithMessage("Telefone é obrigatório")
            .MaximumLength(20).WithMessage("Telefone não pode exceder 20 caracteres");
    }
}
