using Google.Cloud.Firestore;
using Almoxarifado.Domain.Interfaces;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

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
        dict["Id"] = document.Id;

        foreach (var key in dict.Keys.ToList())
        {
            if (dict[key] is Timestamp timestamp)
            {
                dict[key] = timestamp.ToDateTime();
            }
        }

        // CORREÇÃO APLICADA: Permite ler números que estão gravados como texto no banco
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
        };

        var json = JsonSerializer.Serialize(dict, options);
        return JsonSerializer.Deserialize<T>(json, options);
    }

    private Dictionary<string, object> ConverterEntidadeParaDicionario(T entity)
    {
        var dict = new Dictionary<string, object>();
        var propriedades = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in propriedades)
        {
            var valor = prop.GetValue(entity);

            if (valor != null)
            {
                // O Firestore exige que as datas sejam em formato Universal (UTC).
                // Isso garante que ele grave como Timestamp lá no painel.
                if (valor is DateTime data)
                {
                    valor = data.Kind == DateTimeKind.Unspecified
                        ? DateTime.SpecifyKind(data, DateTimeKind.Utc)
                        : data.ToUniversalTime();
                }

                dict[prop.Name] = valor;
            }
        }

        return dict;
    }
}