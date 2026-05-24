namespace Almoxarifado.Domain.Interfaces;

public interface IIdentityService
{
    /// <summary>
    /// Cria um usuário no provedor de identidade (Ex: Firebase) e retorna o UID gerado.
    /// </summary>
    Task<string> CreateUserAsync(string email, string password, string displayName, string role);

    /// <summary>
    /// Remove um usuário do provedor de identidade. Utilizado para rollback.
    /// </summary>
    Task DeleteUserAsync(string uid);
}