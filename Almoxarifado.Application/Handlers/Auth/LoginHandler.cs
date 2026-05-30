using Almoxarifado.Application.Commands.Auth;
using Almoxarifado.Domain.Interfaces;
using MediatR;

namespace Almoxarifado.Application.Handlers.Auth;

public sealed class LoginHandler(IIdentityService identityService) : IRequestHandler<LoginCommand, string>
{
    public Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
        => identityService.AuthenticateAsync(request.Email, request.Password);
}
