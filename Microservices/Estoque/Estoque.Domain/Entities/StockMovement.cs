using Estoque.Domain.Enums;

namespace Estoque.Domain.Entities;

public sealed class StockMovement
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public MovementType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string IdempotencyKey { get; private set; } = string.Empty;

    private StockMovement() { }

    public StockMovement(Guid productId, int quantity, MovementType type, string idempotencyKey)
    {
        if (quantity <= 0) throw new ArgumentException("Quantidade deve ser maior que zero.");
        if (string.IsNullOrWhiteSpace(idempotencyKey)) throw new ArgumentException("Chave de idempotência é obrigatória.");

        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = quantity;
        Type = type;
        IdempotencyKey = idempotencyKey;
        CreatedAt = DateTime.UtcNow;
    }
}
