namespace Pedido.Application.DTOs;

public record OrderDto(Guid Id, int UserId, DateTime CreatedAt, string Status, IEnumerable<OrderItemDto> Items);
public record OrderItemDto(Guid ProductId, int Quantity, decimal UnitPrice);
