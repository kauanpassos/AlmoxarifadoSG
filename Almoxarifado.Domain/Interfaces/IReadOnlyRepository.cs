namespace Almoxarifado.Domain.Interfaces;
public interface IReadOnlyRepository<T> where T : class
{
    Task<T?> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetByFieldAsync(string fieldName, object value);
}
