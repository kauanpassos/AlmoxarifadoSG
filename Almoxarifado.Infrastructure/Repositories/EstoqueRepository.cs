using Almoxarifado.API.Repositories;
using Almoxarifado.Domain.Entities;
using Almoxarifado.Domain.Interfaces;
using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Almoxarifado.Infrastructure.Repositories;

public sealed class EstoqueRepository : IReadOnlyRepository<Produto>
{
    private readonly FirebaseEngine<Produto> _engine;

    public EstoqueRepository(FirestoreDb firestoreDb)
    {
        _engine = new FirebaseEngine<Produto>(firestoreDb, "Produtos");
    }

    public async Task<Produto?> GetByIdAsync(string id) => await _engine.GetByIdAsync(id);

    public async Task<IEnumerable<Produto>> GetAllAsync() => await _engine.GetAllAsync();

    public async Task AddAsync(Produto entity) => await _engine.AddAsync(entity);

    public async Task UpdateAsync(Produto entity) => await _engine.UpdateAsync(entity.Id, entity);

    public async Task DeleteAsync(string id) => await _engine.DeleteAsync(id);
}