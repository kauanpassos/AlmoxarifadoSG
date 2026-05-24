using Almoxarifado.API.Repositories;
using Almoxarifado.Domain.Interfaces;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Almoxarifado.Infrastructure.Repositories;

public class FirebaseRepository<T> : IReadOnlyRepository<T>, IWriteOnlyRepository<T> where T : class
{
    private readonly FirebaseEngine<T> _engine;

    public FirebaseRepository(FirebaseClient firebase)
    {
        var node = typeof(T).Name.ToLower();
        _engine = new FirebaseEngine<T>(firebase, node);
    }

    public Task<T?> GetByIdAsync(string id) => _engine.GetByIdAsync(id);

    public Task<IEnumerable<T>> GetAllAsync() => _engine.GetAllAsync();

    public Task AddAsync(T entity) => _engine.AddAsync(entity);

    public async Task UpdateAsync(T entity)
    {
        var idProp = typeof(T).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
        if (idProp == null)
            throw new InvalidOperationException($"O tipo {typeof(T).Name} precisa expor uma propriedade pública chamada 'Id'.");

        var idValue = idProp.GetValue(entity)?.ToString();
        if (string.IsNullOrWhiteSpace(idValue))
            throw new InvalidOperationException($"O valor do Id não pode ser nulo ou vazio para {typeof(T).Name}.");

        await _engine.UpdateAsync(idValue, entity);
    }

    public Task DeleteAsync(string id) => _engine.DeleteAsync(id);
}