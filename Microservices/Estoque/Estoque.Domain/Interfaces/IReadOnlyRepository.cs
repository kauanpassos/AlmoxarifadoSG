namespace Estoque.Domain.Interfaces;

public interface IReadOnlyRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync(int page, int pageSize);
    Task<bool> ExistsAsync(Guid id);
}
