namespace Almoxarifado.Domain.Interfaces;
public interface IWriteOnlyRepository<T> where T : class
{
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(string id);
}
