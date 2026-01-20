using FluentValidation;
using Lab05.Application.Commands;

namespace Lab05.Application.Validators;

/// <summary>
/// Validador para o comando de exclusão de produto
/// </summary>
public class DeleteProdutoCommandValidator : AbstractValidator<DeleteProdutoCommand>
{
    public DeleteProdutoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");
    }
}
