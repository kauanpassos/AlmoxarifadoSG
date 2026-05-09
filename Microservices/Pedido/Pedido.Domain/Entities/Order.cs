using Pedido.Domain.Enums;

namespace Pedido.Domain.Entities;

public sealed class Order
{
    public Guid Id { get; private set; }
    public int UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public OrderStatus Status { get; private set; }
    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { }

    public Order(int userId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        Status = OrderStatus.Pending;
    }

    public void AddItem(Guid productId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0) throw new ArgumentException("Quantidade deve ser positiva.");
        _items.Add(new OrderItem(Id, productId, quantity, unitPrice));
    }

    public void Confirm() => Status = OrderStatus.Confirmed;
    public void Cancel() => Status = OrderStatus.Cancelled;
}
