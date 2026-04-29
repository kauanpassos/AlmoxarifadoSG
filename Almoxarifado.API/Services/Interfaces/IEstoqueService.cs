using Almoxarifado.Domain;

namespace Almoxarifado.API.Services.Interfaces;

public interface IEstoqueService
{
    Task<IEnumerable<Estoque>> ObterTodos();
}