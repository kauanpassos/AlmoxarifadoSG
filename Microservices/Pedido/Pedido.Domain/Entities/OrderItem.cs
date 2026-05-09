namespace Pedido.Domain.Entities;

public sealed class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    private OrderItem() { }

    internal OrderItem(Guid orderId, Guid productId, int quantity, decimal unitPrice)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}
