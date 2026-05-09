using Pedido.Application.DTOs;
using Pedido.Domain.Entities;

namespace Pedido.Application.Handlers;

// Implementação da lógica de negócio para o CreateOrderHandler.
public sealed partial class CreateOrderHandler
{
    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Validação cross-service para reserva de estoque antes da confirmação do pedido.
        var order = new Order(request.UserId);

        foreach (var item in request.Items)
        {
            var hasStock = await estoqueService.ValidateAndReserveStockAsync(item.ProductId, item.Quantity);
            if (!hasStock) throw new InvalidOperationException($"Estoque insuficiente para o produto {item.ProductId}.");

            order.AddItem(item.ProductId, item.Quantity, item.UnitPrice);
        }

        // Persistência via motor genérico configurado no Domain.
        order.Confirm();
        await repository.AddAsync(order);

        // Conversão para DTO para evitar vazamento de entidades de domínio para a camada de apresentação.
        return new OrderDto(
            order.Id, 
            order.UserId, 
            order.CreatedAt, 
            order.Status.ToString(),
            order.Items.Select(i => new OrderItemDto(i.ProductId, i.Quantity, i.UnitPrice))
        );
    }
}
