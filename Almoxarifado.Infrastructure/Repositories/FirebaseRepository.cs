using Almoxarifado.API.Repositories;
using Almoxarifado.Domain.Interfaces;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Almoxarifado.Infrastructure.Repositories;

public sealed class FirebaseRepository<T>(FirestoreDb firestoreDb) : IReadOnlyRepository<T>, IWriteOnlyRepository<T> where T : class
{
    private readonly FirebaseEngine<T> _engine = new(firestoreDb, typeof(T).Name.ToLower());

    public Task<T?> GetByIdAsync(string id) => _engine.GetByIdAsync(id);

    public Task<IEnumerable<T>> GetAllAsync() => _engine.GetAllAsync();

    public Task<IEnumerable<T>> GetByFieldAsync(string fieldName, object value) => _engine.GetByFieldAsync(fieldName, value);

    public Task AddAsync(T entity) => _engine.AddAsync(entity);

    public Task UpdateAsync(T entity) => _engine.UpdateAsync(entity);

    public Task DeleteAsync(string id) => _engine.DeleteAsync(id);
}