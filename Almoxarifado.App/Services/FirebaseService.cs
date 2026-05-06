using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.Domain;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Almoxarifado.App.Services
{
    public class FirebaseService : IFirebaseService
    {
        private const string ColecaoProdutos = "Produtos";
        private readonly HttpClient _httpClient;
        private readonly string _projectId;
        private readonly string _apiKey;

        public FirebaseService(HttpClient httpClient, string projectId, string apiKey)
        {
            _httpClient = httpClient;
            _projectId = projectId;
            _apiKey = apiKey;
        }

        public async Task<List<Produto>> GetProdutosAsync()
        {
            var listaProdutos = new List<Produto>();
            try
            {
                var url = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents/{ColecaoProdutos}";
                var resp = await _httpClient.GetAsync(url);
                if (!resp.IsSuccessStatusCode)
                    return listaProdutos;

                var json = await resp.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                if (!doc.RootElement.TryGetProperty("documents", out var documents))
                    return listaProdutos;

                foreach (var document in documents.EnumerateArray())
                {
                    var id = document.GetProperty("name").GetString();
                    var parts = id.Split('/');
                    var docId = parts[^1];

                    var hasFields = document.TryGetProperty("fields", out var fields);

                    string GetString(string key)
                        => hasFields && fields.TryGetProperty(key, out var v) && v.TryGetProperty("stringValue", out var s) ? s.GetString() ?? string.Empty : string.Empty;

                    var produto = new Produto(
                        id: docId,
                        nome: GetString("Nome"),
                        marca: GetString("Marca"),
                        sku: GetString("SKU"),
                        unidade: GetString("Unidade")
                    );

                    if (hasFields && fields.TryGetProperty("Ativo", out var ativoEl) && ativoEl.TryGetProperty("booleanValue", out var ativoVal) && !ativoVal.GetBoolean())
                    {
                        produto.Desativar();
                    }

                    listaProdutos.Add(produto);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return listaProdutos;
        }

        public async Task SalvarProdutoAsync(Produto produto)
        {
            try
            {
                var novoId = produto.Id;
                var docName = $"projects/{_projectId}/databases/(default)/documents/{ColecaoProdutos}/{novoId}";
                var url = $"https://firestore.googleapis.com/v1/{docName}";

                var dados = new Dictionary<string, object>
                {
                    { "Nome", new Dictionary<string, object> { { "stringValue", produto.Nome } } },
                    { "Marca", new Dictionary<string, object> { { "stringValue", produto.Marca } } },
                    { "SKU", new Dictionary<string, object> { { "stringValue", produto.Sku } } },
                    { "Unidade", new Dictionary<string, object> { { "stringValue", produto.Unidade } } },
                    { "Ativo", new Dictionary<string, object> { { "booleanValue", produto.Ativo } } },
                    { "createdAt", new Dictionary<string, object> { { "timestampValue", produto.CreatedAt.ToString("o") } } },
                    { "updatedAt", new Dictionary<string, object> { { "timestampValue", produto.UpdatedAt.ToString("o") } } }
                };

                var payload = new { fields = dados };

                var request = new HttpRequestMessage(HttpMethod.Patch, url)
                {
                    Content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json")
                };

                var resp = await _httpClient.SendAsync(request);
                if (!resp.IsSuccessStatusCode)
                {
                    var err = await resp.Content.ReadAsStringAsync();
                    Console.WriteLine(err);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<Usuario> GetUsuarioAsync(string uid)
        {
            var docName = $"projects/{_projectId}/databases/(default)/documents/Usuarios/{uid}";
            var url = $"https://firestore.googleapis.com/v1/{docName}";

            var resp = await _httpClient.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return null;

            var json = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            if (!doc.RootElement.TryGetProperty("fields", out var fields))
                return null;

            string GetStringField(string name)
                => fields.TryGetProperty(name, out var v) && v.TryGetProperty("stringValue", out var s) ? s.GetString() ?? string.Empty : string.Empty;

            return new Usuario(
                id: uid,
                nome: GetStringField("Nome"),
                email: GetStringField("Email"),
                setor: GetStringField("Setor"),
                tipo: GetStringField("Tipo")
            );
        }

        public async Task<List<Estoque>> GetEstoqueAsync()
        {
            return new List<Estoque>();
        }
    }
}