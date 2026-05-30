using Almoxarifado.Application.Commands.Auth;
using Almoxarifado.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Almoxarifado.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
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

        if (string.IsNullOrEmpty(firebaseUid))
            return Unauthorized("Token inválido ou usuário não identificado.");

        var perfil = await mediator.Send(new GetPerfilUsuarioQuery(firebaseUid));

        if (perfil == null)
            return NotFound("Usuário não encontrado na base de dados.");

        int tipoId = 2;
        if (!string.IsNullOrEmpty(perfil.Tipo) && perfil.Tipo.Equals("Almoxarife", StringComparison.OrdinalIgnoreCase))
        {
            tipoId = 1;
        }

        return Ok(new
        {
            Id = perfil.Id,
            Nome = perfil.Nome,
            Email = perfil.Email,
            Setor = perfil.Setor,
            Tipo = tipoId
        });
    }
}