namespace Pedido.Application.Interfaces;

public interface IEstoqueService
{
    Task<bool> ValidateAndReserveStockAsync(Guid productId, int quantity);
}
