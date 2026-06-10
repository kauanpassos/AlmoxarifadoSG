using Almoxarifado.Application.Commands.Auth;
using Almoxarifado.Application.Queries;
using Almoxarifado.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Almoxarifado.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioCommand command)
    {
        var uid = await mediator.Send(command);

        return Ok(new
        {
            Mensagem = "Usuário registrado com sucesso",
            Uid = uid
        });
    }

    [Authorize]
    [HttpGet("perfil")]
    public async Task<IActionResult> ObterPerfil()
    {
        var firebaseUid = User.FindFirst("user_id")?.Value;

        if (string.IsNullOrWhiteSpace(firebaseUid))
            return Unauthorized("Token inválido ou usuário não identificado.");

        var perfil = await mediator.Send(new GetPerfilUsuarioQuery(firebaseUid));

        if (perfil is null)
            return NotFound("Usuário não encontrado na base de dados.");

        var tipoId = string.Equals(perfil.Tipo, "Almoxarife", StringComparison.OrdinalIgnoreCase)
            ? (int)TipoUsuario.Almoxarife
            : (int)TipoUsuario.Colaborador;

        return Ok(new
        {
            perfil.Id,
            perfil.Nome,
            perfil.Email,
            perfil.Setor,
            Tipo = tipoId
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var token = await mediator.Send(command);

        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized(new { Mensagem = "Credenciais inválidas ou erro ao autenticar no Firebase." });

        return Ok(new { Token = token });
    }
}