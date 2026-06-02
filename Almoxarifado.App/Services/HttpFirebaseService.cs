using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Almoxarifado.App.Services;

public class HttpFirebaseService : IFirebaseService
{
    private readonly HttpClient _httpClient;

    public HttpFirebaseService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<List<Produto>> GetProdutosAsync()
    {
        // REMOVIDO o try/catch que "engolia" a falha. 
        // Agora, se der erro de IP ou de servidor, ele joga o erro real para a tela!
        var resposta = await _httpClient.GetAsync("api/produtos");

        if (!resposta.IsSuccessStatusCode)
        {
            throw new Exception($"Erro do Servidor. Status Code: {resposta.StatusCode}");
        }

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var produtos = await resposta.Content.ReadFromJsonAsync<List<Produto>>(options);
        return produtos ?? new List<Produto>();
    }

    public async Task<Usuario> GetUsuarioAsync(string uid)
    {
        var resposta = await _httpClient.GetAsync($"api/usuarios/{uid}");

        if (resposta.IsSuccessStatusCode)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var usuario = await resposta.Content.ReadFromJsonAsync<Usuario>(options);
            return usuario ?? throw new Exception("Usuário retornado nulo pela API.");
        }

        throw new Exception($"Erro na API ao obter usuário: {resposta.StatusCode}");
    }

    public async Task SalvarProdutoAsync(Produto produto)
    {
        var resposta = await _httpClient.PostAsJsonAsync("api/produtos", produto);
        resposta.EnsureSuccessStatusCode();
    }

    public async Task EnviarSolicitacaoAsync(string usuarioId, string sku, long quantidade, string justificativa, string idToken)
    {
        var payload = new
        {
            UsuarioId = usuarioId,
            Sku = sku,
            Quantidade = quantidade,
            Justificativa = justificativa
        };

        var resposta = await _httpClient.PostAsJsonAsync("api/solicitacoes", payload);
        resposta.EnsureSuccessStatusCode();
    }
}