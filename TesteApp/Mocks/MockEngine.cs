using Almoxarifado.Domain.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TesteApp.Mocks;

public class MockEngine<T> : IEngine<T> where T : class
{
    private readonly ConcurrentDictionary<string, T> _data = new();

    public Task<T?> GetByIdAsync(string id)
    {
        _data.TryGetValue(id, out var value);
        return Task.FromResult(value);
    }

    public Task<IEnumerable<T>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<T>>(_data.Values);
    }

    public Task<IEnumerable<T>> GetByFieldAsync(string fieldName, object value)
    {
        var prop = typeof(T).GetProperty(
            fieldName,
            BindingFlags.IgnoreCase |
            BindingFlags.Public |
            BindingFlags.Instance);

        if (prop == null)
        {
            return Task.FromResult<IEnumerable<T>>(Enumerable.Empty<T>());
        }

        var result = _data.Values.Where(item =>
        {
            var propValue = prop.GetValue(item);
            return Equals(propValue, value);
        });

        return Task.FromResult<IEnumerable<T>>(result.ToList());
    }

    public Task AddAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var idProperty = typeof(T).GetProperty("Id");
        var id = idProperty?.GetValue(entity)?.ToString();

        if (string.IsNullOrWhiteSpace(id))
        {
            id = Guid.NewGuid().ToString();
            idProperty?.SetValue(entity, id);
        }

        _data[id] = entity;

        return Task.CompletedTask;
    }

    public Task UpdateAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var idProperty = typeof(T).GetProperty("Id");
        var id = idProperty?.GetValue(entity)?.ToString();

        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException(
                $"Não é possível atualizar um registro de {typeof(T).Name} sem uma propriedade 'Id' válida.");
        }

        _data[id] = entity;

        return Task.CompletedTask;
    }

    public Task DeleteAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return Task.CompletedTask;

        _data.TryRemove(id, out _);

        return Task.CompletedTask;
    }
}