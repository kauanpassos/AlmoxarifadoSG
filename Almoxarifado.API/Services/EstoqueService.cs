using Almoxarifado.API.Services.Interfaces;
using Almoxarifado.Domain;

namespace Almoxarifado.API.Services;

public class EstoqueService : IEstoqueService
{
    public async Task<IEnumerable<Estoque>> ObterTodos()
    {
        return await Task.FromResult(new List<Estoque>());
    }
}