using Almoxarifado.Domain;
using Firebase.Database;

namespace Almoxarifado.API.Repositories;


public sealed class PedidoRepository
{
    private readonly FirebaseEngine<Pedido> _engine;

    public PedidoRepository(FirebaseClient firebase) 
    {
        _engine = new FirebaseEngine<Pedido>(firebase, "pedidos");
    }

    public async Task<Pedido?> GetByIdAsync(int id) => await _engine.GetByIdAsync(id);
    public async Task<IEnumerable<Pedido>> GetAllAsync() => await _engine.GetAllAsync();
    public async Task AddAsync(Pedido entity) => await _engine.AddAsync(entity);
    public async Task UpdateAsync(Pedido entity) => await _engine.UpdateAsync(entity.Id.ToString(), entity);
    public async Task DeleteAsync(int id) => await _engine.DeleteAsync(id);
    public async Task<IEnumerable<Pedido>> GetByUserIdAsync(int userId)
    {
        var response = await _engine.GetAllAsync();
        return response?.Where(p => p.UsuarioId == userId).ToList() ?? Enumerable.Empty<Pedido>();
    }
}
