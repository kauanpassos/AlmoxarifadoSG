using Almoxarifado.API.Repositories;
using Almoxarifado.Domain.Entities;
using Firebase.Database;

namespace Almoxarifado.Infrastructure.Repositories;

public sealed class EstoqueRepository
{
    private readonly FirebaseEngine<Estoque> _engine;

    public EstoqueRepository(FirebaseClient firebase)
    {
        _engine = new FirebaseEngine<Estoque>(firebase, "estoque");
    }

    public async Task<Estoque?> GetByIdAsync(int id) => await _engine.GetByIdAsync(id.ToString());

    public async Task<IEnumerable<Estoque>> GetAllAsync() => await _engine.GetAllAsync();

    public async Task AddAsync(Estoque entity) => await _engine.AddAsync(entity);

    public async Task UpdateAsync(Estoque entity) => await _engine.UpdateAsync(entity.Id.ToString(), entity);

    public async Task DeleteAsync(int id) => await _engine.DeleteAsync(id.ToString());
}