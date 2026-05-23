using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Almoxarifado.Domain;
using Almoxarifado.Application.Commands;
using Almoxarifado.Application.Queries;
namespace Almoxarifado.API.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class SolicitacaoController : ControllerBase
{
    private readonly IMediator _mediator;
    public SolicitacaoController(IMediator mediator) => _mediator = mediator;
    [HttpPost]
    [ProducesResponseType(typeof(Solicitacao), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<ActionResult<Solicitacao>> Post([FromBody] Solicitacao solicitacao)
    {
        var result = await _mediator.Send(new CreateSolicitacaoCommand(solicitacao));
        return Ok(result);
    }
    [HttpGet("usuario/{usuarioId}")]
    [ProducesResponseType(typeof(IEnumerable<Solicitacao>), 200)]
    public async Task<ActionResult<IEnumerable<Solicitacao>>> GetByUserId(string usuarioId)
    {
        var result = await _mediator.Send(new GetSolicitacoesByUserQuery(usuarioId));
        return Ok(result);
    }
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Solicitacao>), 200)]
    public async Task<ActionResult<IEnumerable<Solicitacao>>> GetAll()
    {
        var result = await _mediator.Send(new GetTodasSolicitacoesQuery());
        return Ok(result);
    }
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(Solicitacao), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult<Solicitacao>> AtualizarStatus(string id, [FromBody] AtualizarStatusRequest request)
    {
        var result = await _mediator.Send(new AtualizarStatusSolicitacaoCommand(id, request.NovoStatus));
        return Ok(result);
    }
}
public record AtualizarStatusRequest(string NovoStatus);