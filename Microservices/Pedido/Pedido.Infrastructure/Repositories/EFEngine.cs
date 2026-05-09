using Microsoft.EntityFrameworkCore;
using Pedido.Domain.Interfaces;
using Pedido.Infrastructure.Data;
using System.Linq.Expressions;

namespace Pedido.Infrastructure.Repositories;

public sealed class EFEngine<T>(PedidoDbContext context, params Expression<Func<T, object>>[] includes) : IEngine<T> where T : class
{
    private IQueryable<T> Query => includes.Aggregate(context.Set<T>().AsQueryable(), (current, include) => current.Include(include));

    public async Task<T?> GetByIdAsync(Guid id) => await Query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);

    public async Task AddAsync(T entity)
    {
        await context.Set<T>().AddAsync(entity);
        await context.SaveChangesAsync();
    }
}
