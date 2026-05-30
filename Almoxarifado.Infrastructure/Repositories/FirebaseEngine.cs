using Google.Cloud.Firestore;
using Almoxarifado.Domain.Interfaces;
using System.Text.Json;

namespace Almoxarifado.API.Repositories;

public sealed class FirebaseEngine<T>(FirestoreDb firestoreDb, string collectionName) : IEngine<T> where T : class
{
    private CollectionReference Collection => firestoreDb.Collection(collectionName);

    public async Task<T?> GetByIdAsync(string id)
    {
        var snapshot = await Collection.Document(id).GetSnapshotAsync();
        if (!snapshot.Exists) return null;

        return ConverterDocumentoParaEntidade(snapshot);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var snapshot = await Collection.GetSnapshotAsync();

        return snapshot.Documents
            .Where(d => d.Exists)
            .Select(ConverterDocumentoParaEntidade)
            .Where(e => e != null)!;
    }

    public async Task AddAsync(T entity)
    {
        var idProperty = typeof(T).GetProperty("Id");
        var id = idProperty?.GetValue(entity)?.ToString();

        var dicionario = ConverterEntidadeParaDicionario(entity);

        if (!string.IsNullOrEmpty(id))
        {
            await Collection.Document(id).SetAsync(dicionario);
        }
        else
        {
            var docRef = await Collection.AddAsync(dicionario);
            if (idProperty != null && idProperty.CanWrite)
            {
                idProperty.SetValue(entity, docRef.Id);
            }
        }
    }

    public async Task UpdateAsync(string key, T entity)
    {
        var dicionario = ConverterEntidadeParaDicionario(entity);
        await Collection.Document(key).SetAsync(dicionario, SetOptions.Overwrite);
    }

    public async Task DeleteAsync(string id)
    {
        await Collection.Document(id).DeleteAsync();
    }

    public Task UpdateAsync(T entity)
    {
        var idProperty = typeof(T).GetProperty("Id");
        var id = idProperty?.GetValue(entity)?.ToString();

        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("A entidade não possui um Id válido para atualização.");

        return UpdateAsync(id, entity);
    }

    private T? ConverterDocumentoParaEntidade(DocumentSnapshot document)
    {
        var dict = document.ToDictionary();
        var json = JsonSerializer.Serialize(dict);
        return JsonSerializer.Deserialize<T>(json);
    }

    private Dictionary<string, object> ConverterEntidadeParaDicionario(T entity)
    {
        var json = JsonSerializer.Serialize(entity);
        return JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new Dictionary<string, object>();
    }
}