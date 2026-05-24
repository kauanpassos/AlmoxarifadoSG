using Almoxarifado.Domain.Enums;

namespace Almoxarifado.App.Dtos;

public record LoginRequest(string Email, string Senha);

public record RegistroRequest(string Nome, string Email, string Senha, string Setor, string Tipo);

public record AuthResponse(
    string Id,
    string Nome,
    string Email,
    string Setor,
    TipoUsuario Tipo,
    string Token
);