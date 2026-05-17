using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Almoxarifado.API;
using Almoxarifado.API.Configuration;

namespace TesteApp.IntegrationTests;

public class CorrelationIdTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CorrelationIdTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Request_WithoutCorrelationId_ShouldGenerateAndReturnNewId()
    {
        var response = await _client.GetAsync("/api/estoque");
        var correlationId = GetCorrelationIdFromResponse(response);

        Assert.False(string.IsNullOrWhiteSpace(correlationId));
        Assert.True(Guid.TryParse(correlationId, out _));
    }

    [Fact]
    public async Task Request_WithCorrelationId_ShouldReturnSameId()
    {
        var existingCorrelationId = Guid.NewGuid().ToString();
        _client.DefaultRequestHeaders.Add(HttpConstants.CorrelationIdHeader, existingCorrelationId);

        var response = await _client.GetAsync("/api/estoque");
        var returnedCorrelationId = GetCorrelationIdFromResponse(response);

        Assert.Equal(existingCorrelationId, returnedCorrelationId);
    }

    private static string? GetCorrelationIdFromResponse(HttpResponseMessage response)
    {
        Assert.True(response.Headers.Contains(HttpConstants.CorrelationIdHeader));
        return response.Headers.GetValues(HttpConstants.CorrelationIdHeader).FirstOrDefault();
    }
}
