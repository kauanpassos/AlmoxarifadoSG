using Almoxarifado.Application.Commands.Auth;
using Almoxarifado.Domain.Entities;
using Almoxarifado.Domain.Interfaces;
using MediatR;

namespace Almoxarifado.Application.Handlers.Auth;

public sealed class RegistrarUsuarioHandler(
    IIdentityService identityService,
    IWriteOnlyRepository<Usuario> repository)
    : IRequestHandler<RegistrarUsuarioCommand, string>
{
    public async Task<string> Handle(RegistrarUsuarioCommand request, CancellationToken cancellationToken)
    {
        var uid = await identityService.CreateUserAsync(
            request.Email,
            request.Senha,
            request.Nome,
            request.Tipo.ToString()
        );

        try
        {
            var usuario = new Usuario(
                id: uid,
                nome: request.Nome,
                email: request.Email,
                setor: request.Setor,
                tipo: request.Tipo
            );

            await repository.AddAsync(usuario);

            return uid;
        }
        catch (Exception)
        {
            await identityService.DeleteUserAsync(uid);

            throw new InvalidOperationException("Falha ao salvar o perfil do usuário no banco de dados. A criação no Firebase foi revertida de forma segura.");
        }
    }
}