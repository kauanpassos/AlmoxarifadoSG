using Firebase.Database;
using Firebase.Database.Query;
using Almoxarifado.Domain.Interfaces;
namespace Almoxarifado.API.Repositories;
public sealed class FirebaseEngine<T>(FirebaseClient firebase, string childName) : IEngine<T> where T : class
{
    private ChildQuery Node => firebase.Child(childName);
    public async Task<T?> GetByIdAsync(string id) => await Node.Child(id).OnceSingleAsync<T>();
    public async Task<IEnumerable<T>> GetAllAsync() => (await Node.OnceAsync<T>()).Select(e => e.Object).ToList();
    public Task AddAsync(T entity) => Node.PostAsync(entity);
    public Task UpdateAsync(string key, T entity) => Node.Child(key).PutAsync(entity);
    public Task DeleteAsync(string id) => Node.Child(id).DeleteAsync();
    public Task UpdateAsync(T entity)
    {
        var idProperty = typeof(T).GetProperty("Id");
        var id = idProperty?.GetValue(entity)?.ToString() ?? string.Empty;
        return UpdateAsync(id, entity);
    }
}
