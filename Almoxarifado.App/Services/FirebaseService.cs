using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.Domain;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Almoxarifado.App.Services
{
    public record FirestoreDocumentList<T>(
        [property: JsonPropertyName("documents")] List<T>? Documents);
    public record FirestoreDocumentProduto(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("fields")] FirestoreProdutoFields? Fields);
    public record FirestoreDocumentEstoque(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("fields")] FirestoreEstoqueFields? Fields);
    public record FirestoreEstoqueFields(
        [property: JsonPropertyName("NomePeca")] FirestoreStringField? NomePeca,
        [property: JsonPropertyName("DescricaoTecnica")] FirestoreStringField? DescricaoTecnica,
        [property: JsonPropertyName("Quantidade")] FirestoreIntField? Quantidade,
        [property: JsonPropertyName("Localizacao")] FirestoreStringField? Localizacao);
    public record FirestoreProdutoFields(
        [property: JsonPropertyName("Nome")] FirestoreStringField? Nome,
        [property: JsonPropertyName("Marca")] FirestoreStringField? Marca,
        [property: JsonPropertyName("SKU")] FirestoreStringField? SKU,
        [property: JsonPropertyName("Unidade")] FirestoreStringField? Unidade,
        [property: JsonPropertyName("Ativo")] FirestoreBoolField? Ativo);
    public record FirestoreStringField(
        [property: JsonPropertyName("stringValue")] string? StringValue);
    public record FirestoreIntField(
        [property: JsonPropertyName("integerValue")] string? IntegerValue);
    public record FirestoreBoolField(
        [property: JsonPropertyName("booleanValue")] bool BooleanValue);
    public class FirebaseService : IFirebaseService
    {
        private const string ColecaoProdutos = "Produtos";
        private const string ColecaoEstoque = "Estoque";
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
            var lista = new List<Produto>();
            var url = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents/{ColecaoProdutos}";
            var resp = await _httpClient.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return lista;
            var json = await resp.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<FirestoreDocumentList<FirestoreDocumentProduto>>(json);
            if (result?.Documents == null) return lista;
            foreach (var doc in result.Documents)
            {
                var parts = doc.Name.Split('/');
                var docId = parts[^1];
                var f = doc.Fields;
                if (f == null) continue;
                var produto = new Produto(
                    id: docId,
                    nome: f.Nome?.StringValue ?? string.Empty,
                    marca: f.Marca?.StringValue ?? string.Empty,
                    sku: f.SKU?.StringValue ?? string.Empty,
                    unidade: f.Unidade?.StringValue ?? string.Empty
                );
                if (f.Ativo?.BooleanValue == false)
                    produto.Desativar();
                lista.Add(produto);
            }
            return lista;
        }
        public async Task SalvarProdutoAsync(Produto produto)
        {
            var docName = $"projects/{_projectId}/databases/(default)/documents/{ColecaoProdutos}/{produto.Id}";
            var url = $"https://firestore.googleapis.com/v1/{docName}";
            var dados = new
            {
                fields = new Dictionary<string, object>
                {
                    { "Nome", new { stringValue = produto.Nome } },
                    { "Marca", new { stringValue = produto.Marca } },
                    { "SKU", new { stringValue = produto.Sku } },
                    { "Unidade", new { stringValue = produto.Unidade } },
                    { "Ativo", new { booleanValue = produto.Ativo } },
                    { "createdAt", new { timestampValue = produto.CreatedAt.ToString("o") } },
                    { "updatedAt", new { timestampValue = produto.UpdatedAt.ToString("o") } }
                }
            };
            var request = new HttpRequestMessage(HttpMethod.Patch, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(dados), Encoding.UTF8, "application/json")
            };
            var resp = await _httpClient.SendAsync(request);
            if (!resp.IsSuccessStatusCode)
            {
                var err = await resp.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Falha ao salvar produto no Firestore: {err}");
            }
        }
        public async Task<Usuario> GetUsuarioAsync(string uid)
        {
            var url = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents/Usuarios/{uid}";
            var resp = await _httpClient.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                throw new KeyNotFoundException($"Usuário '{uid}' não encontrado.");
            var json = await resp.Content.ReadAsStringAsync();
            var doc = JsonSerializer.Deserialize<FirestoreDocumentResponse>(json);
            if (doc?.Fields == null)
                throw new InvalidOperationException("Dados do usuário estão incompletos no Firestore.");
            return new Usuario(
                id: uid,
                nome: doc.Fields.Nome?.StringValue ?? string.Empty,
                email: doc.Fields.Email?.StringValue ?? string.Empty,
                setor: doc.Fields.Setor?.StringValue ?? string.Empty,
                tipo: doc.Fields.Tipo?.StringValue ?? string.Empty
            );
        }
        public async Task<List<Estoque>> GetEstoqueAsync()
        {
            var lista = new List<Estoque>();
            var url = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents/{ColecaoEstoque}";
            var resp = await _httpClient.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return lista;
            var json = await resp.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<FirestoreDocumentList<FirestoreDocumentEstoque>>(json);
            if (result?.Documents == null) return lista;
            foreach (var doc in result.Documents)
            {
                var parts = doc.Name.Split('/');
                var rawId = parts[^1];
                var f = doc.Fields;
                if (f == null) continue;
                _ = int.TryParse(rawId, out var numericId);
                _ = int.TryParse(f.Quantidade?.IntegerValue, out var quantidade);
                lista.Add(new Estoque
                {
                    Id = numericId,
                    NomePeca = f.NomePeca?.StringValue ?? string.Empty,
                    DescricaoTecnica = f.DescricaoTecnica?.StringValue ?? string.Empty,
                    Quantidade = quantidade,
                    Localizacao = f.Localizacao?.StringValue ?? string.Empty
                });
            }
            return lista;
        }
        public async Task EnviarSolicitacaoAsync(string usuarioId, string sku, int quantidade, string justificativa, string idToken)
        {
            var solicitacaoId = Guid.NewGuid().ToString();
            var docName = $"projects/{_projectId}/databases/(default)/documents/Solicitacoes/{solicitacaoId}";
            var url = $"https://firestore.googleapis.com/v1/{docName}";
            var dados = new
            {
                fields = new Dictionary<string, object>
                {
                    { "Id", new { stringValue = solicitacaoId } },
                    { "UsuarioId", new { stringValue = usuarioId } },
                    { "Sku", new { stringValue = sku } },
                    { "Quantidade", new { integerValue = quantidade.ToString() } },
                    { "Observacao", new { stringValue = justificativa } },
                    { "Status", new { stringValue = "Pendente" } },
                    { "CreatedAt", new { timestampValue = DateTime.UtcNow.ToString("o") } },
                    { "UpdatedAt", new { timestampValue = DateTime.UtcNow.ToString("o") } }
                }
            };
            var request = new HttpRequestMessage(HttpMethod.Patch, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(dados), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);
            var resp = await _httpClient.SendAsync(request);
            if (!resp.IsSuccessStatusCode)
            {
                var err = await resp.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Falha ao registrar solicitação: {err}");
            }
        }
    }
}