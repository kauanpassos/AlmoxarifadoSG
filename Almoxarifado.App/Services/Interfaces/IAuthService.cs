using Almoxarifado.Domain.Entities;

namespace Almoxarifado.App.Services.Interfaces;

public interface IAuthService
{
    Task<Usuario?> LoginAsync(string email, string password);
    Task<Usuario?> VerificarSessaoAtivaAsync();
    Task LogoutAsync();
}