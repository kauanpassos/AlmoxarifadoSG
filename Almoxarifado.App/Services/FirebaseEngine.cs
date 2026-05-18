using Firebase.Database;
using Firebase.Database.Query;
using Almoxarifado.Domain.Interfaces;
using Polly;
using Polly.Retry;

namespace Almoxarifado.App.Services;
public sealed class FirebaseEngine<T>(FirebaseClient firebase, string childName) : IEngine<T> where T : class
{
    private ChildQuery Node => firebase.Child(childName);

    private readonly AsyncRetryPolicy _retryPolicy = Policy
        .Handle<Exception>()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    public Task<T?> GetByIdAsync(int id) 
        => _retryPolicy.ExecuteAsync(async () => await Node.Child(id.ToString()).OnceSingleAsync<T>());

    public async Task<IEnumerable<T>> GetAllAsync() 
        => await _retryPolicy.ExecuteAsync(async () => (await Node.OnceAsync<T>()).Select(e => e.Object).ToList());

    public Task AddAsync(T entity) 
        => _retryPolicy.ExecuteAsync(() => Node.PostAsync(entity));

    public Task UpdateAsync(string key, T entity) 
        => _retryPolicy.ExecuteAsync(() => Node.Child(key).PutAsync(entity));

    public Task DeleteAsync(int id) 
        => _retryPolicy.ExecuteAsync(() => Node.Child(id.ToString()).DeleteAsync());

    public Task UpdateAsync(T entity)
    {
        var idProperty = typeof(T).GetProperty("Id");
        var id = idProperty?.GetValue(entity)?.ToString() ?? string.Empty;
        return UpdateAsync(id, entity);
    }
}
