using Microsoft.AspNetCore.Mvc;
using MediatR;
using Pedido.Application.Commands;
using Pedido.Application.DTOs;

namespace Pedido.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Post([FromBody] CreateOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
