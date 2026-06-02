using System;
using System.Threading.Tasks;
using Xunit;

namespace TesteApp.Fixtures;

public sealed partial class DatabaseFixture : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await ClearAllData();
        await SeedTestData();
    }

    public async Task DisposeAsync() => await ClearAllData();

    public async Task ClearAllData()
    {
        try
        {
            // Alterado de _estoqueEngine para _produtoEngine
            await _produtoEngine.DeleteAsync("0");
        }
        catch { }
    }
}