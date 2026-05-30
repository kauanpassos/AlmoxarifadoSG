namespace Almoxarifado.Domain.Interfaces;

public interface IIdentityService
{
    Task<string> CreateUserAsync(string email, string password, string displayName, string role);
    Task DeleteUserAsync(string uid);
    Task<string> AuthenticateAsync(string email, string password);
}