using Firebase.Database;
using Firebase.Database.Query;
using Almoxarifado.Domain.Interfaces;

namespace Almoxarifado.API.Repositories;

// Motor genérico para operações no Firebase.
// Centraliza a lógica de acesso técnico, permitindo que novas entidades sejam adicionadas sem novos repositórios.
public sealed class FirebaseEngine<T>(FirebaseClient firebase, string childName) : IEngine<T> where T : class
{
    // Node centralizado para evitar repetição de caminhos (DRY Principle).
    private ChildQuery Node => firebase.Child(childName);

    // Membros de expressão (=>) utilizados para reduzir verbosidade em métodos de delegação simples.
    public async Task<T?> GetByIdAsync(int id) => await Node.Child(id.ToString()).OnceSingleAsync<T>();

    public async Task<IEnumerable<T>> GetAllAsync() => (await Node.OnceAsync<T>()).Select(e => e.Object).ToList();

    public Task AddAsync(T entity) => Node.PostAsync(entity);

    public Task UpdateAsync(string key, T entity) => Node.Child(key).PutAsync(entity);

    public Task DeleteAsync(int id) => Node.Child(id.ToString()).DeleteAsync();

    // Sobrecarga técnica para suporte a interfaces de escrita que não exigem chave explícita.
    public Task UpdateAsync(T entity)
    {
        var idProperty = typeof(T).GetProperty("Id");
        var id = idProperty?.GetValue(entity)?.ToString() ?? string.Empty;
        return UpdateAsync(id, entity);
    }
}
