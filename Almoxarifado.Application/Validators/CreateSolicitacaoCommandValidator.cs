using FluentValidation;
using Almoxarifado.Application.Commands;

namespace Almoxarifado.Application.Validators;

public sealed class CreateSolicitacaoCommandValidator : AbstractValidator<CreateSolicitacaoCommand>
{
    public CreateSolicitacaoCommandValidator()
    {
        RuleFor(x => x.UsuarioId)
            .NotEmpty().WithMessage("O ID do usuário solicitante é obrigatório.");

        RuleFor(x => x.Itens)
            .NotEmpty().WithMessage("A solicitação deve conter pelo menos um item.");

        RuleForEach(x => x.Itens).ChildRules(itens =>
        {
            itens.RuleFor(i => i.Sku).NotEmpty().WithMessage("O SKU do item é obrigatório.");
            itens.RuleFor(i => i.Quantidade).GreaterThan(0).WithMessage("A quantidade do item deve ser maior que zero.");
        });
    }
}