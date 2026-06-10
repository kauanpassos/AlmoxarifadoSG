using System.Collections.Generic;
using System.Threading.Tasks;

namespace Almoxarifado.Domain.Interfaces;

/// <summary>
/// Contrato genérico e somente-leitura para consultas à camada de persistência de dados.
/// </summary>
/// <typeparam name="T">Entidade de domínio mapeada pelo repositório.</typeparam>
public interface IReadOnlyRepository<T> where T : class
{
    Task<T?> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetByFieldAsync(string fieldName, object value);
}