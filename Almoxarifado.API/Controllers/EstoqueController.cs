using Microsoft.AspNetCore.Mvc;
using MediatR;
using Almoxarifado.Application.Queries;
using Almoxarifado.Domain.Entities;

namespace Almoxarifado.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class EstoqueController : ControllerBase
{
    private readonly IMediator _mediator;

    public EstoqueController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Estoque>>> Get()
    {
        var query = new GetEstoqueQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}