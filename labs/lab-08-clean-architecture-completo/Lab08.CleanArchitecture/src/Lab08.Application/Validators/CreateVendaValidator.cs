using FluentValidation;
using Lab08.Application.UseCases.Vendas.CreateVenda;

namespace Lab08.Application.Validators;

/// <summary>
/// Validador para CreateVendaInput
/// </summary>
public class CreateVendaValidator : AbstractValidator<CreateVendaInput>
{
    public CreateVendaValidator()
    {
        RuleFor(x => x.ClienteId)
            .GreaterThan(0).WithMessage("Cliente é obrigatório");

        RuleFor(x => x.Itens)
            .NotEmpty().WithMessage("A venda deve ter pelo menos um item")
            .Must(itens => itens.All(i => i.ProdutoId > 0))
            .WithMessage("Todos os itens devem ter um produto válido")
            .Must(itens => itens.All(i => i.Quantidade > 0))
            .WithMessage("Todos os itens devem ter quantidade maior que zero");

        RuleForEach(x => x.Itens)
            .SetValidator(new ItemVendaInputValidator());

        RuleFor(x => x.Observacao)
            .MaximumLength(500).WithMessage("Observação deve ter no máximo 500 caracteres");
    }
}

/// <summary>
/// Validador para ItemVendaInput
/// </summary>
public class ItemVendaInputValidator : AbstractValidator<ItemVendaInput>
{
    public ItemVendaInputValidator()
    {
        RuleFor(x => x.ProdutoId)
            .GreaterThan(0).WithMessage("Produto é obrigatório");

        RuleFor(x => x.Quantidade)
            .GreaterThan(0).WithMessage("Quantidade deve ser maior que zero")
            .LessThanOrEqualTo(1000).WithMessage("Quantidade máxima por item é 1000");
    }
}
