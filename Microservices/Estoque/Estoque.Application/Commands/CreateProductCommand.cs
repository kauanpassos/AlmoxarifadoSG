using MediatR;
using Estoque.Application.DTOs;

namespace Estoque.Application.Commands;

public record CreateProductCommand(string Name, string TechnicalDescription, int InitialQuantity, string Location) : IRequest<ProductDto>;
