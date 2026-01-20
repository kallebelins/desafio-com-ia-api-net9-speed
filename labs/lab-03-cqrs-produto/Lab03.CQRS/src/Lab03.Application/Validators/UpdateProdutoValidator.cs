using FluentValidation;
using Lab03.Application.Commands;

namespace Lab03.Application.Validators;

/// <summary>
/// Validador para o comando UpdateProdutoCommand
/// </summary>
public class UpdateProdutoValidator : AbstractValidator<UpdateProdutoCommand>
{
    public UpdateProdutoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("O ID do produto é obrigatório");

        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("O nome do produto é obrigatório")
            .MaximumLength(200)
            .WithMessage("O nome do produto não pode ter mais de 200 caracteres");

        RuleFor(x => x.Preco)
            .GreaterThan(0)
            .WithMessage("O preço deve ser maior que zero");

        RuleFor(x => x.Categoria)
            .NotEmpty()
            .WithMessage("A categoria é obrigatória")
            .MaximumLength(100)
            .WithMessage("A categoria não pode ter mais de 100 caracteres");

        RuleFor(x => x.Estoque)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O estoque não pode ser negativo");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000)
            .WithMessage("A descrição não pode ter mais de 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descricao));
    }
}
