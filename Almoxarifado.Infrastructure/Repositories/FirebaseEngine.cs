using Google.Cloud.Firestore;
using Almoxarifado.Domain.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace Almoxarifado.API.Repositories;

public sealed class FirebaseEngine<T>(FirestoreDb firestoreDb, string collectionName) : IEngine<T> where T : class
{
    private CollectionReference Collection => firestoreDb.Collection(collectionName);

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        TypeInfoResolver = new System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver
        {
            Modifiers = { ModifyTypeInfo }
        }
    };

    public async Task<T?> GetByIdAsync(string id)
    {
        var snapshot = await Collection.Document(id).GetSnapshotAsync();

        if (snapshot.Exists is false)
            return default;

        return ConverterDocumentoParaEntidade(snapshot);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var snapshot = await Collection.GetSnapshotAsync();
        return ExtrairEntidadesDaSnapshot(snapshot);
    }

    public async Task<IEnumerable<T>> GetByFieldAsync(string fieldName, object value)
    {
        var query = Collection.WhereEqualTo(fieldName, value);
        var snapshot = await query.GetSnapshotAsync();

        return ExtrairEntidadesDaSnapshot(snapshot);
    }

    public async Task AddAsync(T entity)
    {
        var idProperty = typeof(T).GetProperty("Id");
        var id = ObterValorDaPropriedadeId(entity, idProperty);
        var dicionario = ConverterEntidadeParaDicionario(entity);

        if (string.IsNullOrWhiteSpace(id) is false)
        {
            await Collection.Document(id).SetAsync(dicionario);
            return;
        }

        var docRef = await Collection.AddAsync(dicionario);

        if (idProperty is not null && idProperty.CanWrite)
        {
            idProperty.SetValue(entity, docRef.Id);
        }
    }

    public async Task UpdateAsync(string key, T entity)
    {
        var dicionario = ConverterEntidadeParaDicionario(entity);
        await Collection.Document(key).SetAsync(dicionario, SetOptions.Overwrite);
    }

    public Task UpdateAsync(T entity)
    {
        var idProperty = typeof(T).GetProperty("Id");
        var id = ObterValorDaPropriedadeId(entity, idProperty);

        if (string.IsNullOrWhiteSpace(id))
            throw new InvalidOperationException("Não é possível atualizar a entidade porque o identificador (Id) está nulo ou vazio.");

        return UpdateAsync(id, entity);
    }

    public async Task DeleteAsync(string id)
    {
        await Collection.Document(id).DeleteAsync();
    }

    private IEnumerable<T> ExtrairEntidadesDaSnapshot(QuerySnapshot snapshot)
    {
        return snapshot.Documents
            .Where(d => d.Exists)
            .Select(ConverterDocumentoParaEntidade)
            .Where(e => e is not null)!;
    }

    private static string? ObterValorDaPropriedadeId(T entity, PropertyInfo? idProperty)
    {
        return idProperty?.GetValue(entity)?.ToString();
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

        var json = JsonSerializer.Serialize(dict, _jsonOptions);
        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }

    private static void ModifyTypeInfo(System.Text.Json.Serialization.Metadata.JsonTypeInfo ti)
    {
        if (ti.Kind is not System.Text.Json.Serialization.Metadata.JsonTypeInfoKind.Object)
            return;

        foreach (var property in ti.Properties)
        {
            if (property.Set is null)
            {
                var propInfo = ti.Type.GetProperty(property.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (propInfo is not null && propInfo.CanWrite)
                {
                    property.Set = propInfo.SetValue;
                }
            }
        }

        foreach (var field in ti.Type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
        {
            var propName = field.Name.TrimStart('_');
            if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                var itemType = field.FieldType.GetGenericArguments()[0];
                var jsonProp = ti.CreateJsonPropertyInfo(typeof(IEnumerable<>).MakeGenericType(itemType), propName);

                jsonProp.Set = (obj, value) =>
                {
                    var list = field.GetValue(obj);
                    var addRange = field.FieldType.GetMethod("AddRange");
                    if (list is not null && value is not null && addRange is not null)
                    {
                        addRange.Invoke(list, new[] { value });
                    }
                };

                if (!ti.Properties.Any(p => p.Name.Equals(propName, StringComparison.OrdinalIgnoreCase)))
                {
                    ti.Properties.Add(jsonProp);
                }
            }
        }
    }

    private Dictionary<string, object> ConverterEntidadeParaDicionario(T entity)
    {
        var dict = new Dictionary<string, object>();
        var propriedades = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in propriedades)
        {
            if (prop.GetCustomAttribute<JsonIgnoreAttribute>() is not null)
                continue;

            var valor = prop.GetValue(entity);

            if (valor is not null)
            {
                if (valor is DateTime data)
                {
                    valor = data.Kind == DateTimeKind.Unspecified
                        ? DateTime.SpecifyKind(data, DateTimeKind.Utc)
                        : data.ToUniversalTime();
                }

                var firestorePropAttr = prop.GetCustomAttribute<FirestorePropertyAttribute>();
                var jsonPropAttr = prop.GetCustomAttribute<JsonPropertyNameAttribute>();
                var nomeCampo = prop.Name;

                if (firestorePropAttr is not null && !string.IsNullOrWhiteSpace(firestorePropAttr.Name))
                {
                    nomeCampo = firestorePropAttr.Name;
                }
                else if (jsonPropAttr is not null && !string.IsNullOrWhiteSpace(jsonPropAttr.Name))
                {
                    nomeCampo = jsonPropAttr.Name;
                }

                dict[nomeCampo] = valor;
            }
        }

        return dict;
    }
}