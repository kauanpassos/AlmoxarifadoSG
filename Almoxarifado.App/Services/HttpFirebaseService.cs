using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Linq;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.Application.DTOs;
using Microsoft.Maui.Storage;

namespace Almoxarifado.App.Services;

public sealed class HttpFirebaseService : IFirebaseService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public HttpFirebaseService(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }
    private async Task GarantirAutenticacaoAsync()
    {
        var token = await SecureStorage.Default.GetAsync("auth_token");
        if (!string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<IEnumerable<ProdutoDto>> GetProdutosAsync()
    {
        await GarantirAutenticacaoAsync();
        var resposta = await _httpClient.GetAsync("api/produtos");

        if (resposta.IsSuccessStatusCode is false)
            throw new HttpRequestException($"Falha na comunicação com o servidor de produtos. Código: {resposta.StatusCode}");

        var produtos = await resposta.Content.ReadFromJsonAsync<List<ProdutoDto>>(_jsonOptions);

        return produtos ?? new List<ProdutoDto>();
    }

    public async Task<UsuarioDto> GetUsuarioAsync(string uid)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uid);

        await GarantirAutenticacaoAsync();
        var resposta = await _httpClient.GetAsync($"api/usuarios/{uid}");

        if (resposta.IsSuccessStatusCode is false)
            throw new HttpRequestException($"Não foi possível obter os dados do usuário. Código do servidor: {resposta.StatusCode}");

        var usuario = await resposta.Content.ReadFromJsonAsync<UsuarioDto>(_jsonOptions);

        if (usuario is null)
            throw new InvalidOperationException("O servidor retornou um perfil de usuário vazio ou inválido.");

        return usuario;
    }

    public async Task SalvarProdutoAsync(ProdutoDto produto)
    {
        ArgumentNullException.ThrowIfNull(produto);

        await GarantirAutenticacaoAsync();
        var resposta = await _httpClient.PostAsJsonAsync("api/produtos", produto);

        if (resposta.IsSuccessStatusCode is false)
            throw new HttpRequestException($"Falha ao registrar o produto na base remota. Código: {resposta.StatusCode}");
    }

    public async Task EnviarSolicitacaoLoteAsync(string usuarioId, string observacao, IEnumerable<CartItemDto> itens)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(usuarioId);
        ArgumentNullException.ThrowIfNull(itens);

        if (itens is not ICollection<CartItemDto> { Count: > 0 })
            throw new ArgumentException("A lista de itens solicitados não pode estar vazia.", nameof(itens));

        var payload = new
        {
            UsuarioId = usuarioId,
            Observacao = observacao,
            Itens = itens.Select(i => new
            {
                Sku = i.Sku,
                NomeProduto = i.NomeProduto,
                Quantidade = i.Quantidade
            }).ToList()
        };

        await GarantirAutenticacaoAsync();
        var resposta = await _httpClient.PostAsJsonAsync("api/solicitacoes", payload);

        if (resposta.IsSuccessStatusCode is false)
            throw new HttpRequestException($"O lote de solicitações foi recusado pelo servidor. Código: {resposta.StatusCode}");
    }

    public async Task<IEnumerable<SolicitacaoDto>> GetSolicitacoesUsuarioAsync(string usuarioId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(usuarioId);

        await GarantirAutenticacaoAsync();
        var resposta = await _httpClient.GetAsync($"api/solicitacoes/usuario/{usuarioId}");

        if (resposta.IsSuccessStatusCode is false)
            throw new HttpRequestException($"Erro ao carregar as solicitações do usuário. Código: {resposta.StatusCode}");

        var solicitacoes = await resposta.Content.ReadFromJsonAsync<List<SolicitacaoDto>>(_jsonOptions);

        return solicitacoes ?? new List<SolicitacaoDto>();
    }

    public async Task<IEnumerable<SolicitacaoDto>> GetSolicitacoesPendentesAsync()
    {
        await GarantirAutenticacaoAsync();
        var resposta = await _httpClient.GetAsync("api/solicitacoes/pendentes");

        if (resposta.IsSuccessStatusCode is false)
            throw new HttpRequestException($"Erro ao carregar o catálogo de solicitações pendentes. Código: {resposta.StatusCode}");

        var solicitacoes = await resposta.Content.ReadFromJsonAsync<List<SolicitacaoDto>>(_jsonOptions);

        return solicitacoes ?? new List<SolicitacaoDto>();
    }

    public async Task AtualizarStatusSolicitacaoAsync(string id, string novoStatus)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(novoStatus);

        var payload = new { NovoStatus = novoStatus };

        await GarantirAutenticacaoAsync();
        var resposta = await _httpClient.PatchAsJsonAsync($"api/solicitacoes/{id}/status", payload);

        if (resposta.IsSuccessStatusCode is false)
            throw new HttpRequestException($"Falha ao atualizar status da solicitação. Código: {resposta.StatusCode}");
    }
}