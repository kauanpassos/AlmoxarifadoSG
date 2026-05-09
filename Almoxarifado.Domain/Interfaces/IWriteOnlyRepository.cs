namespace Almoxarifado.Domain.Interfaces;

// Interface dedicada exclusivamente à escrita e modificação de dados.
// Use esta interface em Handlers que realizam ações de criação, edição ou deleção.
public interface IWriteOnlyRepository<T> where T : class
{
    // Adiciona uma nova entidade ao banco.
    Task AddAsync(T entity);
    
    // Atualiza um registro existente.
    Task UpdateAsync(T entity);
    
    // Remove um registro pelo ID.
    Task DeleteAsync(int id);
}
