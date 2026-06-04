using Almoxarifado.Domain.Interfaces;
using System.Collections.Concurrent;
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
        var result = _data.Values.Where(item => 
        {
            var prop = typeof(T).GetProperty(fieldName, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (prop == null) return false;
            var propValue = prop.GetValue(item);
            return object.Equals(propValue, value);
        });
        return Task.FromResult<IEnumerable<T>>(result.ToList());
    }
    public Task AddAsync(T entity)
    {
        var idProperty = typeof(T).GetProperty("Id");
        var id = idProperty?.GetValue(entity)?.ToString() ?? Guid.NewGuid().ToString();
        _data[id] = entity;
        return Task.CompletedTask;
    }
    public Task UpdateAsync(T entity)
    {
        var idProperty = typeof(T).GetProperty("Id");
        var id = idProperty?.GetValue(entity)?.ToString() ?? string.Empty;
        _data[id] = entity;
        return Task.CompletedTask;
    }
    public Task DeleteAsync(string id)
    {
        _data.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}
