using Almoxarifado.App.ViewModels;
using FluentValidation;

namespace Almoxarifado.App.Validations;

public sealed class CadastroValidator : AbstractValidator<CadastroViewModel>
{
    public CadastroValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MinimumLength(3).WithMessage("O nome deve ter pelo menos 3 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("O e-mail informado não é válido.");

        RuleFor(x => x.Setor)
            .NotEmpty().WithMessage("O setor é obrigatório.");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(6).WithMessage("A senha deve ter pelo menos 6 caracteres.");

        RuleFor(x => x.ConfirmarSenha)
            .NotEmpty().WithMessage("A confirmação de senha é obrigatória.")
            .Equal(x => x.Senha).WithMessage("As senhas não coincidem.");
    }
}
