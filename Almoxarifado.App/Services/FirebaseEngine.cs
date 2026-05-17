using Firebase.Database;
using Firebase.Database.Query;
using Almoxarifado.Domain.Interfaces;
using Polly;
using Polly.Retry;

namespace Almoxarifado.App.Services;

// Motor Firebase com resiliência integrada (Polly).
// Implementa políticas de Retry para garantir que oscilações de rede não quebrem a UX do Mobile.
public sealed class FirebaseEngine<T>(FirebaseClient firebase, string childName) : IEngine<T> where T : class
{
    private ChildQuery Node => firebase.Child(childName);

    // Política de Retry: Tenta 3 vezes com espera exponencial (2s, 4s, 8s).
    private readonly AsyncRetryPolicy _retryPolicy = Policy
        .Handle<Exception>() // Em um cenário real, filtraríamos apenas exceções de rede/timeout.
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
