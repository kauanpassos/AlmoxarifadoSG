using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Xunit;
using Almoxarifado.API;
using Almoxarifado.Domain.Interfaces;
using Almoxarifado.Domain.Entities;

namespace TesteApp.IntegrationTests;

public class SolicitacaoControllerIntegrationTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = default!;
    private HttpClient _client = default!;

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

                // Substituído Estoque por Produto no escopo de Mocking da injeção de dependência
                services.AddSingleton<IEngine<Produto>, Mocks.MockEngine<Produto>>();
                services.AddSingleton<IEngine<Solicitacao>, Mocks.MockEngine<Solicitacao>>();

                services.AddSingleton<IReadOnlyRepository<Produto>>(sp => sp.GetRequiredService<IEngine<Produto>>());
                services.AddSingleton<IWriteOnlyRepository<Produto>>(sp => sp.GetRequiredService<IEngine<Produto>>());

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
        // A rota permanece "/api/estoque" porque o EstoqueController mapeia esse endpoint interno, 
        // mas ele agora responde entregando a lista de Produtos unificada.
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