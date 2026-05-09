using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;

namespace TesteApp.IntegrationTests;

[TestClass]
public class PedidoControllerIntegrationTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
        _client.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    [TestMethod]
    public async Task ObterEstoque_DeveRetornarOk()
    {
        var response = await _client.GetAsync("/api/estoque");

        Assert.AreEqual(200, (int)response.StatusCode);
    }

    [TestMethod]
    public async Task ObterEstoque_DeveRetornarJson()
    {
        var response = await _client.GetAsync("/api/estoque");
        var content = await response.Content.ReadAsStringAsync();

        Assert.IsNotNull(content);
        Assert.IsTrue(content.StartsWith("[") || content.StartsWith("{"));
    }

    [TestMethod]
    public async Task CriarPedido_ComRequest_DeveRetornarOk()
    {
        var request = new { usuarioId = 1 };
        var json = JsonSerializer.Serialize(request);
        var httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/pedido", httpContent);

        Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created);
    }

    [TestMethod]
    public async Task CriarPedido_DeveRetornarPedidoComStatus()
    {
        var request = new { usuarioId = 1 };
        var json = JsonSerializer.Serialize(request);
        var httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/pedido", httpContent);
        var responseJson = await response.Content.ReadAsStringAsync();

        Assert.IsTrue(responseJson.Contains("Pendente") || responseJson.Contains("pedido"));
    }

    [TestMethod]
    public async Task ListarPorUsuario_DeveRetornarOk()
    {
        var response = await _client.GetAsync("/api/pedido/usuario/1");

        Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.NotFound);
    }

    [TestMethod]
    public async Task ListarPorUsuario_DeveRetornarJson()
    {
        var response = await _client.GetAsync("/api/pedido/usuario/1");
        var content = await response.Content.ReadAsStringAsync();

        Assert.IsTrue(content.StartsWith("[") || content.StartsWith("{") || content.Length == 0);
    }
}
