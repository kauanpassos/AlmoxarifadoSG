using Estoque.Domain.Entities;

namespace Estoque.Domain.Interfaces;

public interface IProductRepository : IReadOnlyRepository<Product>, IWriteOnlyRepository<Product>
{
}
