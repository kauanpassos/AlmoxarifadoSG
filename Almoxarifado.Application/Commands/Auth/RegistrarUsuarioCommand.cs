using Almoxarifado.Domain.Enums;
using MediatR;

namespace Almoxarifado.Application.Commands.Auth;
public record RegistrarUsuarioCommand(
    string Nome,
    string Email,
    string Senha,
    string Setor,
    TipoUsuario Tipo
) : IRequest<string>;