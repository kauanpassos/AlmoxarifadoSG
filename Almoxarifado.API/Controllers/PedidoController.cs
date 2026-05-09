using Microsoft.AspNetCore.Mvc;
using MediatR;
using Almoxarifado.Domain;
using Almoxarifado.Application.Commands;
using Almoxarifado.Application.Queries;

namespace Almoxarifado.API.Controllers;

/// <summary>
/// Controller responsável pela gestão de pedidos de almoxarifado.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class PedidoController : ControllerBase
{
    private readonly IMediator _mediator;

    public PedidoController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Registra um novo pedido no sistema.
    /// </summary>
    /// <param name="order">Dados do pedido a ser criado.</param>
    /// <returns>O pedido criado com seu status inicial.</returns>
    /// <response code="200">Retorna o pedido recém-criado.</response>
    /// <response code="400">Se o pedido contiver dados inválidos (Validação Automática).</response>
    [HttpPost]
    [ProducesResponseType(typeof(Pedido), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<ActionResult<Pedido>> Post([FromBody] Pedido order)
    {
        var result = await _mediator.Send(new CreatePedidoCommand(order));
        return Ok(result);
    }

    /// <summary>
    /// Recupera o histórico de pedidos de um usuário específico.
    /// </summary>
    /// <param name="usuarioId">ID identificador do usuário.</param>
    /// <returns>Lista de pedidos associados ao usuário.</returns>
    [HttpGet("usuario/{usuarioId:int}")]
    [ProducesResponseType(typeof(IEnumerable<Pedido>), 200)]
    public async Task<ActionResult<IEnumerable<Pedido>>> GetByUserId(int usuarioId)
    {
        var result = await _mediator.Send(new GetPedidosByUserQuery(usuarioId));
        return Ok(result);
    }
}