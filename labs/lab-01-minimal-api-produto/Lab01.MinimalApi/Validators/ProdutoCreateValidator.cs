using FluentValidation;
using Lab01.MinimalApi.ValueObjects;

namespace Lab01.MinimalApi.Validators;

public class ProdutoCreateValidator : AbstractValidator<ProdutoCreateDto>
{
    public ProdutoCreateValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome não pode exceder 200 caracteres");

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("Descrição é obrigatória")
            .MaximumLength(1000).WithMessage("Descrição não pode exceder 1000 caracteres");

        RuleFor(x => x.Preco)
            .GreaterThan(0).WithMessage("Preço deve ser maior que zero");
    }
}
