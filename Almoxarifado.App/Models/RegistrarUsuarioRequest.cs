using Almoxarifado.Domain.Enums;

namespace Almoxarifado.App.Models;

public record RegistrarUsuarioRequest(
    string Nome, 
    string Email, 
    string Senha, 
    string Setor, 
    TipoUsuario Tipo
);
