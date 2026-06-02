using Almoxarifado.App.Models;
using Almoxarifado.Domain.Entities;
using Almoxarifado.Domain.Enums;

namespace Almoxarifado.App.Services.Interfaces;

public interface IAuthService
{
    Task<Usuario?> LoginAsync(string email, string password);
    Task RegistrarAsync(RegistrarUsuarioRequest request);
    Task<Usuario?> VerificarSessaoAtivaAsync();
    Task LogoutAsync();
}