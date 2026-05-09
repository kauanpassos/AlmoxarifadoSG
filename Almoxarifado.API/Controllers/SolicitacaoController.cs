using Microsoft.AspNetCore.Mvc;
using MediatR;
using Almoxarifado.Domain;
using Almoxarifado.Application.Commands;
using Almoxarifado.Application.Queries;

namespace Almoxarifado.API.Controllers;

/// <summary>
/// Controller responsável pela gestão de solicitações de almoxarifado.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class SolicitacaoController : ControllerBase
{
    private readonly IMediator _mediator;

    public SolicitacaoController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Registra uma nova solicitação no sistema.
    /// </summary>
    /// <param name="solicitacao">Dados da solicitação a ser criada.</param>
    /// <returns>A solicitação criada com seu status inicial.</returns>
    /// <response code="200">Retorna a solicitação recém-criada.</response>
    /// <response code="400">Se a solicitação contiver dados inválidos.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Solicitacao), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<ActionResult<Solicitacao>> Post([FromBody] Solicitacao solicitacao)
    {
        var result = await _mediator.Send(new CreateSolicitacaoCommand(solicitacao));
        return Ok(result);
    }

    /// <summary>
    /// Recupera o histórico de solicitações de um usuário específico.
    /// </summary>
    /// <param name="usuarioId">ID identificador do usuário.</param>
    /// <returns>Lista de solicitações associadas ao usuário.</returns>
    [HttpGet("usuario/{usuarioId}")]
    [ProducesResponseType(typeof(IEnumerable<Solicitacao>), 200)]
    public async Task<ActionResult<IEnumerable<Solicitacao>>> GetByUserId(string usuarioId)
    {
        var result = await _mediator.Send(new GetSolicitacoesByUserQuery(usuarioId));
        return Ok(result);
    }
}