namespace Almoxarifado.Domain.Interfaces;

using Almoxarifado.Domain;

public static class PedidoExtensions
{
    public static async Task<IEnumerable<Pedido>> GetByUserIdAsync(this IReadOnlyRepository<Pedido> repository, int userId)
    {
        var all = await repository.GetAllAsync();
        return all.Where(p => p.UsuarioId == userId);
    }
}
