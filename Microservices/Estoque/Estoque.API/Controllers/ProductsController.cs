using Microsoft.AspNetCore.Mvc;
using MediatR;
using Estoque.Application.Commands;
using Estoque.Domain.Interfaces;
using Estoque.Application.DTOs;

namespace Estoque.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IProductRepository _repository;

    public ProductsController(IMediator mediator, IProductRepository repository)
    {
        _mediator = mediator;
        _repository = repository;
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Post([FromBody] CreateProductCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null) return NotFound("Produto não encontrado.");

        return Ok(new ProductDto(product.Id, product.Name, product.TechnicalDescription, product.Quantity, product.Location));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var products = await _repository.GetAllAsync(page, pageSize);
        var dtos = products.Select(p => new ProductDto(p.Id, p.Name, p.TechnicalDescription, p.Quantity, p.Location));
        return Ok(dtos);
    }
}
