using FluentValidation;
using Almoxarifado.Application.Commands;

namespace Almoxarifado.Application.Validators;

// Validador especializado para o comando de criação de solicitações.
public sealed class CreateSolicitacaoCommandValidator : AbstractValidator<CreateSolicitacaoCommand>
{
    public CreateSolicitacaoCommandValidator()
    {
        RuleFor(x => x.Solicitacao)
            .NotNull().WithMessage("A solicitação não pode ser nula.");

        RuleFor(x => x.Solicitacao.UsuarioId)
            .NotEmpty().WithMessage("O ID do usuário solicitante é obrigatório.");

        RuleFor(x => x.Solicitacao.Status)
            .Equal("Pendente").WithMessage("Uma nova solicitação deve iniciar com o status 'Pendente'.");
    }
}
