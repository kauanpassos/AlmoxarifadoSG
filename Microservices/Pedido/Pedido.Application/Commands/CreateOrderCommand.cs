using MediatR;
using Pedido.Application.DTOs;

namespace Pedido.Application.Commands;

public record CreateOrderCommand(int UserId, List<OrderItemDto> Items) : IRequest<OrderDto>;
