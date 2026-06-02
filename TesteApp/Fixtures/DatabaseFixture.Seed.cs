using Almoxarifado.Domain.Entities;
using System.Threading.Tasks;

namespace TesteApp.Fixtures;

public sealed partial class DatabaseFixture
{
    public async Task SeedTestData()
    {
        var produtos = new[]
        {
            new Produto("1", 1001, "Correia", "Correia de transmissão", "A1", 50),
            new Produto("2", 1002, "Parafuso", "Parafuso M10", "A2", 200),
            new Produto("3", 1003, "Polia", "Polia 30mm", "B1", 15)
        };

        foreach (var produto in produtos)
        {
            await _produtoEngine.AddAsync(produto);
        }
    }
}