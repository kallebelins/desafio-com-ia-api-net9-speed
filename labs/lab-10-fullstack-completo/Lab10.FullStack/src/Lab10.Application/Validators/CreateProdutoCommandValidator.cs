using FluentValidation;
using Lab10.Application.Commands.Produtos;

namespace Lab10.Application.Validators;

public class CreateProdutoCommandValidator : AbstractValidator<CreateProdutoCommand>
{
    public CreateProdutoCommandValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

        RuleFor(x => x.PrecoUnitario)
            .GreaterThan(0).WithMessage("Preço unitário deve ser maior que zero");

        RuleFor(x => x.EstoqueInicial)
            .GreaterThanOrEqualTo(0).WithMessage("Estoque inicial não pode ser negativo");

        RuleFor(x => x.CategoriaId)
            .GreaterThan(0).WithMessage("Categoria é obrigatória");
    }
}
