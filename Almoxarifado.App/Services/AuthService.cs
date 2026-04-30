using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.Domain;
using System.Threading.Tasks;
using System;

namespace Almoxarifado.App.Services
{
    public class AuthService : IAuthService
    {
        public async Task<Usuario?> LoginAsync(string email, string password)
        {
            try
            {
                if (email.ToLower().Contains("almoxarife"))
                {
                    return new Usuario(
                        id: "123",
                        nome: "Almoxarife Teste",
                        email: email,
                        setor: "Almoxarifado",
                        tipo: "Almoxarife"
                    );
                }
                else
                {
                    return new Usuario(
                        id: "456",
                        nome: "Funcionário Teste",
                        email: email,
                        setor: "Operação",
                        tipo: "Funcionario"
                    );
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}