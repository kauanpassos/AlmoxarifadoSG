namespace Estoque.Application.DTOs;

public record ProductDto(Guid Id, string Name, string TechnicalDescription, int Quantity, string Location);
