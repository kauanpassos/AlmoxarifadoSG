using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Almoxarifado.Application.Commands;
using Almoxarifado.Application.Queries;
using Almoxarifado.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Almoxarifado.API.Controllers;

[ApiController]
[Route("api/solicitacoes")]
[Authorize]
public sealed class SolicitacaoController : ControllerBase
{
    private readonly IMediator _mediator;

    public SolicitacaoController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [ProducesResponseType(typeof(SolicitacaoDto), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<IActionResult> Post([FromBody] CreateSolicitacaoCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SolicitacaoDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult<SolicitacaoDto>> GetById(string id)
    {
        var result = await _mediator.Send(new GetSolicitacaoByIdQuery(id));
        return Ok(result);
    }

    [HttpGet("usuario/{usuarioId}")]
    [ProducesResponseType(typeof(IEnumerable<SolicitacaoDto>), 200)]
    public async Task<ActionResult<IEnumerable<SolicitacaoDto>>> GetByUserId(string usuarioId)
    {
        var result = await _mediator.Send(new GetSolicitacoesByUserQuery(usuarioId));
        return Ok(result);
    }

    [HttpGet]
    [Authorize(Policy = "AlmoxarifeOnly")]
    [ProducesResponseType(typeof(IEnumerable<SolicitacaoDto>), 200)]
    public async Task<ActionResult<IEnumerable<SolicitacaoDto>>> GetAll()
    {
        var result = await _mediator.Send(new GetTodasSolicitacoesQuery());
        return Ok(result);
    }

    [HttpGet("pendentes")]
    [Authorize(Policy = "AlmoxarifeOnly")]
    [ProducesResponseType(typeof(IEnumerable<SolicitacaoDto>), 200)]
    public async Task<ActionResult<IEnumerable<SolicitacaoDto>>> GetPendentes()
    {
        var pendentes = await _mediator.Send(new GetSolicitacoesPendentesQuery());
        return Ok(pendentes);
    }

    [HttpPatch("{id}/status")]
    [Authorize(Policy = "AlmoxarifeOnly")]
    [ProducesResponseType(typeof(SolicitacaoDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult<SolicitacaoDto>> AtualizarStatus(string id, [FromBody] AtualizarStatusRequest request)
    {
        var result = await _mediator.Send(new AtualizarStatusSolicitacaoCommand(id, request.NovoStatus));
        return Ok(result);
    }
}

public record AtualizarStatusRequest(string NovoStatus);