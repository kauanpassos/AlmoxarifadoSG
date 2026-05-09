using MediatR;
using AutoMapper;
using Estoque.Application.Commands;
using Estoque.Application.DTOs;
using Estoque.Domain.Entities;
using Estoque.Domain.Interfaces;

namespace Estoque.Application.Handlers;

public sealed class CreateProductHandler(IProductRepository repository, IMapper mapper) 
    : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product(request.Name, request.TechnicalDescription, request.InitialQuantity, request.Location);
        await repository.AddAsync(product);
        return mapper.Map<ProductDto>(product);
    }
}
