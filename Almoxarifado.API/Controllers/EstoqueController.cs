using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Almoxarifado.Application.Queries;
using Almoxarifado.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Almoxarifado.API.Controllers;

[ApiController]
[Route("api/produtos")]
[Authorize]
public sealed class EstoqueController : ControllerBase
{
    private readonly IMediator _mediator;

    public EstoqueController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Produto>>> Get()
    {
        var query = new GetEstoqueQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("{id}/entrada")]
    [Authorize(Policy = "AlmoxarifeOnly")]
    [ProducesResponseType(typeof(Almoxarifado.Application.DTOs.ProdutoDto), 200)]
    public async Task<IActionResult> AdicionarEstoque(string id, [FromBody] QuantidadeRequest request)
    {
        var result = await _mediator.Send(new Almoxarifado.Application.Commands.AdicionarEstoqueCommand(id, request.Quantidade));
        return Ok(result);
    }

    [HttpPost("{id}/saida")]
    [Authorize(Policy = "AlmoxarifeOnly")]
    [ProducesResponseType(typeof(Almoxarifado.Application.DTOs.ProdutoDto), 200)]
    public async Task<IActionResult> BaixarEstoque(string id, [FromBody] QuantidadeRequest request)
    {
        var result = await _mediator.Send(new Almoxarifado.Application.Commands.BaixarEstoqueCommand(id, request.Quantidade));
        return Ok(result);
    }
}

public record QuantidadeRequest(long Quantidade);