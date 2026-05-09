using Almoxarifado.Domain;

namespace Almoxarifado.App.Services.Interfaces;

public interface IAuthService
{
    Task<Usuario?> LoginAsync(string email, string password);
    Task LogoutAsync();
}