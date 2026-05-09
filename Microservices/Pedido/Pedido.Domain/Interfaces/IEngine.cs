namespace Pedido.Domain.Interfaces;

public interface IReadOnlyRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
}

public interface IWriteOnlyRepository<T> where T : class
{
    Task AddAsync(T entity);
}

public interface IEngine<T> : IReadOnlyRepository<T>, IWriteOnlyRepository<T> where T : class
{
}
