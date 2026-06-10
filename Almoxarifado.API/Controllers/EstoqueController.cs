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
}