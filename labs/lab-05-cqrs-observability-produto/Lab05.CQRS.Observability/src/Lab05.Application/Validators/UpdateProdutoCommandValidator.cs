using FluentValidation;
using Lab05.Application.Commands;

namespace Lab05.Application.Validators;

/// <summary>
/// Validador para o comando de atualização de produto
/// </summary>
public class UpdateProdutoCommandValidator : AbstractValidator<UpdateProdutoCommand>
{
    public UpdateProdutoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.Preco)
            .GreaterThan(0).WithMessage("Preço deve ser maior que zero");

        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU é obrigatório")
            .MaximumLength(50).WithMessage("SKU deve ter no máximo 50 caracteres")
            .Matches("^[A-Za-z0-9-_]+$").WithMessage("SKU deve conter apenas letras, números, hífen e underscore");

        RuleFor(x => x.Categoria)
            .NotEmpty().WithMessage("Categoria é obrigatória")
            .MaximumLength(100).WithMessage("Categoria deve ter no máximo 100 caracteres");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000).WithMessage("Descrição deve ter no máximo 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descricao));
    }
}
