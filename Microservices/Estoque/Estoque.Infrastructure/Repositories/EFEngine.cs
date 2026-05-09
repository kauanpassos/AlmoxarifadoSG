using Microsoft.EntityFrameworkCore;
using Estoque.Domain.Interfaces;
using Estoque.Infrastructure.Data;

namespace Estoque.Infrastructure.Repositories;

// Motor genérico baseado em Entity Framework Core.
// Este motor é utilizado no microserviço de estoque para persistência em bancos relacionais (SQL).
// Notem que ele implementa a mesma interface IEngine que o Firebase, mas de forma totalmente diferente internamente.
public sealed class EFEngine<T>(EstoqueDbContext context) : IEngine<T> where T : class
{
    // Atalho para o DbSet da entidade T.
    private DbSet<T> Set => context.Set<T>();

    // Busca rápida por chave primária (Guid).
    public async Task<T?> GetByIdAsync(Guid id) => await Set.FindAsync(id);

    // Listagem paginada. Notem o uso de AsNoTracking() para melhorar a performance em consultas de leitura.
    public async Task<IEnumerable<T>> GetAllAsync(int page, int pageSize) => 
        await Set.AsNoTracking().Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

    // Verificação de existência sem carregar o objeto inteiro na memória.
    public async Task<bool> ExistsAsync(Guid id) => await Set.FindAsync(id) != null;

    public async Task AddAsync(T entity)
    {
        await Set.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        Set.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await Set.FindAsync(id);
        if (entity != null)
        {
            Set.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}
