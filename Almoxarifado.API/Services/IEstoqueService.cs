using Almoxarifado.Domain;

namespace Almoxarifado.API.Services;

public interface IEstoqueService
{
    Task<IEnumerable<Estoque>> ObterTodos();
}