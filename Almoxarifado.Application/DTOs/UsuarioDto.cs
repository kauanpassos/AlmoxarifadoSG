using Almoxarifado.Domain.Enums;

namespace Almoxarifado.Application.DTOs;

public record UsuarioDto(
    string Id,
    string Email,
    string Nome,
    string Setor,
    TipoUsuario Tipo);