using Estoque.Domain.Entities;

namespace Estoque.Domain.Interfaces;

public interface IMovementRepository
{
    Task AddAsync(StockMovement movement);
    Task<bool> IdempotencyKeyExistsAsync(string key);
}
