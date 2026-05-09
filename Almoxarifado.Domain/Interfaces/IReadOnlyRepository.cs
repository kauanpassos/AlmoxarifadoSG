namespace Almoxarifado.Domain.Interfaces;

// Interface dedicada exclusivamente à leitura de dados.
// Segregar a leitura da escrita (ISP) nos permite ter serviços que não conseguem alterar o banco por acidente.
public interface IReadOnlyRepository<T> where T : class
{
    // Recupera uma entidade única pelo ID.
    Task<T?> GetByIdAsync(int id);
    
    // Lista todos os registros da coleção.
    Task<IEnumerable<T>> GetAllAsync();
}
