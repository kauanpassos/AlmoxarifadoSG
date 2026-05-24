using Almoxarifado.Application.Commands.Auth;
using FluentValidation;

namespace Almoxarifado.Application.Validators.Auth;

public sealed class RegistrarUsuarioCommandValidator : AbstractValidator<RegistrarUsuarioCommand>
{
    public RegistrarUsuarioCommandValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O Nome é obrigatório.")
            .MinimumLength(3).WithMessage("O Nome deve ter pelo menos 3 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O E-mail é obrigatório.")
            .EmailAddress().WithMessage("Formato de e-mail inválido.");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("A Senha é obrigatória.")
            .MinimumLength(6).WithMessage("A senha deve ter no mínimo 6 caracteres (regra nativa do Firebase).");

        RuleFor(x => x.Setor)
            .NotEmpty().WithMessage("O Setor é obrigatório.");

        RuleFor(x => x.Tipo)
            .IsInEnum().WithMessage("Tipo de usuário inválido.");
    }
}