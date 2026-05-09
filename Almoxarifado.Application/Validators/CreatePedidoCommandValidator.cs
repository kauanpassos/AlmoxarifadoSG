using FluentValidation;
using Almoxarifado.Application.Commands;

namespace Almoxarifado.Application.Validators;

// Validador específico para o comando de criação de pedidos.
// Aqui definimos as regras de negócio de entrada.
public sealed class CreatePedidoCommandValidator : AbstractValidator<CreatePedidoCommand>
{
    public CreatePedidoCommandValidator()
    {
        RuleFor(v => v.Pedido)
            .NotNull().WithMessage("O objeto de pedido é obrigatório.");

        RuleFor(v => v.Pedido.UsuarioId)
            .GreaterThan(0).WithMessage("O ID do usuário deve ser um número positivo válido.");

        RuleFor(v => v.Pedido.Status)
            .NotEmpty().When(v => v.Pedido != null)
            .WithMessage("O status inicial deve ser informado.");
    }
}
