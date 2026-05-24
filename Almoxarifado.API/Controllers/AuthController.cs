using Almoxarifado.Application.Commands.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Almoxarifado.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Registra um novo usuário no Firebase Auth e no banco de dados.
    /// Apenas para a criação de usuários. O Login é feito diretamente pelo client (MAUI).
    /// </summary>
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
}