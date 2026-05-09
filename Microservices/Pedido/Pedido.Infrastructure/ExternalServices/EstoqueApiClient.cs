using System.Net.Http.Json;
using Pedido.Application.Interfaces;
using Polly;
using Polly.Retry;

namespace Pedido.Infrastructure.ExternalServices;

public sealed class EstoqueApiClient : IEstoqueService
{
    private readonly HttpClient _httpClient;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

    public EstoqueApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        
        // Política de Resiliência: Tenta 3 vezes com espera exponencial
        _retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    public async Task<bool> ValidateAndReserveStockAsync(Guid productId, int quantity)
    {
        // Simulando a chamada para o microsserviço de Estoque
        // Na vida real, seria um POST para /api/products/{id}/reserve
        var response = await _retryPolicy.ExecuteAsync(() => 
            _httpClient.PostAsJsonAsync($"api/products/{productId}/reserve", new { quantity }));

        return response.IsSuccessStatusCode;
    }
}
