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
            await _estoqueEngine.DeleteAsync("0");
        }
        catch { }
    }
}
