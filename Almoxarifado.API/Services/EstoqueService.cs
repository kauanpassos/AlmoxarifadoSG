using Almoxarifado.Domain;
using Supabase;

namespace Almoxarifado.API.Services;

public class EstoqueService : IEstoqueService
{
    private readonly Client _supabase;
    public EstoqueService(Client supabase) => _supabase = supabase;

    public async Task<IEnumerable<Estoque>> ObterTodos()
    {
        var response = await _supabase.From<Estoque>().Get();
        return response.Models;
    }
}