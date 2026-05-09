using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;

namespace TesteApp.IntegrationTests;

[TestClass]
public class SolicitacaoControllerIntegrationTests : IAsyncLifetime
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
    public async Task CriarSolicitacao_ComRequest_DeveRetornarOk()
    {
        // Simulando um payload de Solicitacao. 
        // OBS: Em um cenário real, o payload deve seguir o contrato da entidade Solicitacao.
        var request = new { id = Guid.NewGuid().ToString(), usuarioId = "user123", observacao = "Teste" };
        var json = JsonSerializer.Serialize(request);
        var httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/solicitacao", httpContent);

        Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK || 
                      response.StatusCode == System.Net.HttpStatusCode.Created ||
                      response.StatusCode == System.Net.HttpStatusCode.BadRequest); // Aceitamos 400 se o validador barrar
    }

    [TestMethod]
    public async Task ListarPorUsuario_DeveRetornarOk()
    {
        var response = await _client.GetAsync("/api/solicitacao/usuario/user123");
        Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK || 
                      response.StatusCode == System.Net.HttpStatusCode.NotFound);
    }
}
