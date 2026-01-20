using FluentValidation;
using Lab10.Application.Commands.Vendas;

namespace Lab10.Application.Validators;

public class IniciarVendaCommandValidator : AbstractValidator<IniciarVendaCommand>
{
    public IniciarVendaCommandValidator()
    {
        RuleFor(x => x.ClienteId)
            .GreaterThan(0).WithMessage("Cliente é obrigatório");

        RuleFor(x => x.Itens)
            .NotEmpty().WithMessage("A venda deve ter pelo menos um item");

        RuleForEach(x => x.Itens).ChildRules(item =>
        {
            item.RuleFor(i => i.ProdutoId)
                .GreaterThan(0).WithMessage("Produto é obrigatório");

            item.RuleFor(i => i.Quantidade)
                .GreaterThan(0).WithMessage("Quantidade deve ser maior que zero");
        });
    }
}
