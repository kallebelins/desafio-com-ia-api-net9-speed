using FluentValidation;
using Lab08.Application.UseCases.Produtos.CreateProduto;

namespace Lab08.Application.Validators;

/// <summary>
/// Validador para CreateProdutoInput
/// </summary>
public class CreateProdutoValidator : AbstractValidator<CreateProdutoInput>
{
    public CreateProdutoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000).WithMessage("Descrição deve ter no máximo 1000 caracteres");

        RuleFor(x => x.Preco)
            .GreaterThan(0).WithMessage("Preço deve ser maior que zero");

        RuleFor(x => x.Estoque)
            .GreaterThanOrEqualTo(0).WithMessage("Estoque não pode ser negativo");

        RuleFor(x => x.CategoriaId)
            .GreaterThan(0).WithMessage("Categoria é obrigatória");
    }
}
