using MediatR;

namespace Almoxarifado.Application.Commands.Auth;

public sealed record LoginCommand(string Email, string Password) : IRequest<string>;
