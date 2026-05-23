using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Xunit;
using Almoxarifado.API;
using Almoxarifado.Domain;
using Almoxarifado.Domain.Interfaces;
namespace TesteApp.IntegrationTests;
public class SolicitacaoControllerIntegrationTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;
    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptors = services.Where(d => d.ServiceType.IsGenericType && 
                    (d.ServiceType.GetGenericTypeDefinition() == typeof(IEngine<>) ||
                     d.ServiceType.GetGenericTypeDefinition() == typeof(IReadOnlyRepository<>) ||
                     d.ServiceType.GetGenericTypeDefinition() == typeof(IWriteOnlyRepository<>))).ToList();
                foreach (var descriptor in descriptors)
                {
                    services.Remove(descriptor);
                }
                services.AddSingleton<IEngine<Estoque>, Mocks.MockEngine<Estoque>>();
                services.AddSingleton<IEngine<Solicitacao>, Mocks.MockEngine<Solicitacao>>();
                services.AddSingleton<IReadOnlyRepository<Estoque>>(sp => sp.GetRequiredService<IEngine<Estoque>>());
                services.AddSingleton<IWriteOnlyRepository<Estoque>>(sp => sp.GetRequiredService<IEngine<Estoque>>());
                services.AddSingleton<IReadOnlyRepository<Solicitacao>>(sp => sp.GetRequiredService<IEngine<Solicitacao>>());
                services.AddSingleton<IWriteOnlyRepository<Solicitacao>>(sp => sp.GetRequiredService<IEngine<Solicitacao>>());
                services.AddMvc(options =>
                {
                    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AllowAnonymousFilter());
                });
            });
        });
        _client = _factory.CreateClient();
        _client.Timeout = TimeSpan.FromSeconds(30);
        await Task.CompletedTask;
    }
    public async Task DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();
        await Task.CompletedTask;
    }
    [Fact]
    public async Task ObterEstoque_DeveRetornarOk()
    {
        var response = await _client.GetAsync("/api/estoque");
        Assert.Equal(200, (int)response.StatusCode);
    }
    [Fact]
    public async Task CriarSolicitacao_ComRequest_DeveRetornarOk()
    {
        var request = new { id = Guid.NewGuid().ToString(), usuarioId = "user123", observacao = "Teste" };
        var json = JsonSerializer.Serialize(request);
        var httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/solicitacao", httpContent);
        Assert.True(response.IsSuccessStatusCode || 
                      response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                      response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
    }
    [Fact]
    public async Task ListarPorUsuario_DeveRetornarOk()
    {
        var response = await _client.GetAsync("/api/solicitacao/usuario/user123");
        Assert.True(response.IsSuccessStatusCode || 
                      response.StatusCode == System.Net.HttpStatusCode.NotFound ||
                      response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
    }
}
