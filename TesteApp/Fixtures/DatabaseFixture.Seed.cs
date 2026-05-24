using Almoxarifado.Domain;
using Almoxarifado.Domain.Entities;

namespace TesteApp.Fixtures;

public sealed partial class DatabaseFixture
{
    public async Task SeedTestData()
    {
        var estoques = new[]
        {
            new Estoque { Id = 1, NomePeca = "Correia", DescricaoTecnica = "Correia de transmissão", Quantidade = 50, Localizacao = "A1" },
            new Estoque { Id = 2, NomePeca = "Parafuso", DescricaoTecnica = "Parafuso M10", Quantidade = 200, Localizacao = "A2" },
            new Estoque { Id = 3, NomePeca = "Polia", DescricaoTecnica = "Polia 30mm", Quantidade = 15, Localizacao = "B1" }
        };

        foreach (var estoque in estoques)
        {
            await _estoqueEngine.AddAsync(estoque);
        }
    }
}
