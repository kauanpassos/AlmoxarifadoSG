using Almoxarifado.App.Models;
using Almoxarifado.Application.DTOs;
using Almoxarifado.Domain.Enums;
using System.Threading.Tasks;

namespace Almoxarifado.App.Services.Interfaces;

public interface IAuthService
{
    Task<UsuarioDto?> LoginAsync(string email, string password);
    Task RegistrarAsync(RegistrarUsuarioRequest request);
    Task<UsuarioDto?> VerificarSessaoAtivaAsync();
    Task LogoutAsync();
    string? ObterUsuarioIdAtual();
}