using Almoxarifado.API.Repositories;
using Almoxarifado.Domain.Entities;
using Almoxarifado.Domain.Interfaces;
using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Almoxarifado.Infrastructure.Repositories;

public sealed class EstoqueRepository(FirestoreDb firestoreDb) : IReadOnlyRepository<Produto>, IWriteOnlyRepository<Produto>
{
    private readonly FirebaseEngine<Produto> _engine = new(firestoreDb, "Produtos");

    public Task<Produto?> GetByIdAsync(string id) => _engine.GetByIdAsync(id);

    public Task<IEnumerable<Produto>> GetAllAsync() => _engine.GetAllAsync();

    public Task<IEnumerable<Produto>> GetByFieldAsync(string fieldName, object value) => _engine.GetByFieldAsync(fieldName, value);

    public Task AddAsync(Produto entity) => _engine.AddAsync(entity);

    public Task UpdateAsync(Produto entity) => _engine.UpdateAsync(entity);

    public Task DeleteAsync(string id) => _engine.DeleteAsync(id);
}