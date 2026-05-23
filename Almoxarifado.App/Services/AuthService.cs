using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.Domain;
using Firebase.Auth;
using Microsoft.Maui.Storage;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Almoxarifado.App.Services;
public record SignInResponse(
    [property: JsonPropertyName("idToken")] string IdToken,
    [property: JsonPropertyName("localId")] string LocalId,
    [property: JsonPropertyName("email")] string Email);
public record FirestoreStringValue(
    [property: JsonPropertyName("stringValue")] string StringValue);
public record FirestoreUsuarioFields(
    [property: JsonPropertyName("Nome")] FirestoreStringValue Nome,
    [property: JsonPropertyName("Email")] FirestoreStringValue Email,
    [property: JsonPropertyName("Setor")] FirestoreStringValue Setor,
    [property: JsonPropertyName("Tipo")] FirestoreStringValue Tipo);
public record FirestoreDocumentResponse(
    [property: JsonPropertyName("fields")] FirestoreUsuarioFields Fields);
public class AuthService : IAuthService
{
    private readonly FirebaseAuthClient _authClient;
    private readonly HttpClient _httpClient;
    private readonly string _projectId;
    private readonly string _apiKey;
    public AuthService(FirebaseAuthClient authClient, HttpClient httpClient, string projectId, string apiKey)
    {
        _authClient = authClient;
        _httpClient = httpClient;
        _projectId = projectId;
        _apiKey = apiKey;
    }
    public async Task<Usuario?> LoginAsync(string email, string password)
    {
        try
        {
            var signInUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_apiKey}";
            var signInBody = new { email, password, returnSecureToken = true };
            var payload = new StringContent(JsonSerializer.Serialize(signInBody), Encoding.UTF8, "application/json");
            var signInResponse = await _httpClient.PostAsync(signInUrl, payload);
            if (!signInResponse.IsSuccessStatusCode)
                throw new Exception("Credenciais inválidas ou erro de comunicação.");
            var signInJson = await signInResponse.Content.ReadAsStringAsync();
            var signInObj = JsonSerializer.Deserialize<SignInResponse>(signInJson);
            if (signInObj == null || string.IsNullOrEmpty(signInObj.IdToken) || string.IsNullOrEmpty(signInObj.LocalId))
                return null;
            var idToken = signInObj.IdToken;
            var localId = signInObj.LocalId;
            var userEmail = !string.IsNullOrEmpty(signInObj.Email) ? signInObj.Email : email;
            await SecureStorage.Default.SetAsync("auth_token", idToken);
            await SecureStorage.Default.SetAsync("user_uid", localId);
            var docName = $"projects/{_projectId}/databases/(default)/documents/Usuarios/{localId}";
            var url = $"https://firestore.googleapis.com/v1/{docName}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);
            var resp = await _httpClient.SendAsync(request);
            if (!resp.IsSuccessStatusCode) return null;
            var json = await resp.Content.ReadAsStringAsync();
            var doc = JsonSerializer.Deserialize<FirestoreDocumentResponse>(json);
            if (doc?.Fields == null) return null;
            var usuario = new Usuario(
                id: localId,
                nome: doc.Fields.Nome?.StringValue ?? string.Empty,
                email: userEmail ?? doc.Fields.Email?.StringValue ?? string.Empty,
                setor: doc.Fields.Setor?.StringValue ?? string.Empty,
                tipo: doc.Fields.Tipo?.StringValue ?? string.Empty
            );
            UsuarioSessao.UsuarioLogado = usuario;
            return usuario;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro de Autenticação: {ex.Message}");
        }
    }
    public async Task<Usuario?> VerificarSessaoAtivaAsync()
    {
        try
        {
            var idToken = await SecureStorage.Default.GetAsync("auth_token");
            var localId = await SecureStorage.Default.GetAsync("user_uid");
            if (string.IsNullOrEmpty(idToken) || string.IsNullOrEmpty(localId))
                return null;
            var docName = $"projects/{_projectId}/databases/(default)/documents/Usuarios/{localId}";
            var url = $"https://firestore.googleapis.com/v1/{docName}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);
            var resp = await _httpClient.SendAsync(request);
            if (!resp.IsSuccessStatusCode)
            {
                await LogoutAsync();
                return null;
            }
            var json = await resp.Content.ReadAsStringAsync();
            var doc = JsonSerializer.Deserialize<FirestoreDocumentResponse>(json);
            if (doc?.Fields == null) return null;
            var usuario = new Usuario(
                id: localId,
                nome: doc.Fields.Nome?.StringValue ?? string.Empty,
                email: doc.Fields.Email?.StringValue ?? string.Empty,
                setor: doc.Fields.Setor?.StringValue ?? string.Empty,
                tipo: doc.Fields.Tipo?.StringValue ?? string.Empty
            );
            UsuarioSessao.UsuarioLogado = usuario;
            return usuario;
        }
        catch
        {
            return null;
        }
    }
    public async Task LogoutAsync()
    {
        try
        {
            _authClient?.SignOut();
        }
        catch (Exception)
        {
        }
        UsuarioSessao.UsuarioLogado = null;
        SecureStorage.Default.Remove("auth_token");
        SecureStorage.Default.Remove("user_uid");
        await Task.CompletedTask;
    }
    public async Task<Usuario> RegistrarAsync(string nome, string email, string senha, string setor, string tipo)
    {
        Usuario.ValidarRegrasDeSenha(senha);
        try
        {
            var userCredential = await _authClient.CreateUserWithEmailAndPasswordAsync(email, senha);
            var novoId = userCredential.User.Uid;
            var novoUsuario = new Usuario(novoId, nome, email, setor, tipo);
            var dadosUsuario = new Dictionary<string, object>
            {
                { "Nome", novoUsuario.Nome },
                { "Email", novoUsuario.Email },
                { "Setor", novoUsuario.Setor },
                { "Tipo", novoUsuario.Tipo },
                { "Ativo", novoUsuario.Ativo },
                { "CreatedAt", novoUsuario.CreatedAt },
                { "UpdatedAt", novoUsuario.UpdatedAt }
            };
            var signInUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_apiKey}";
            var signInBody = new { email, password = senha, returnSecureToken = true };
            var payload = new StringContent(JsonSerializer.Serialize(signInBody), Encoding.UTF8, "application/json");
            var signInResponse = await _httpClient.PostAsync(signInUrl, payload);
            if (!signInResponse.IsSuccessStatusCode)
            {
                var err = await signInResponse.Content.ReadAsStringAsync();
                throw new Exception($"Falha ao autenticar após criação do usuário: {err}");
            }
            var signInJson = await signInResponse.Content.ReadAsStringAsync();
            var signInObj = JsonSerializer.Deserialize<SignInResponse>(signInJson);
            var idToken = signInObj?.IdToken ?? string.Empty;
            var docName = $"projects/{_projectId}/databases/(default)/documents/Usuarios/{novoId}";
            var url = $"https://firestore.googleapis.com/v1/{docName}?currentDocument.exists=false";
            var firestoreFields = new Dictionary<string, object>();
            foreach (var kv in dadosUsuario)
            {
                if (kv.Value is bool b)
                    firestoreFields[kv.Key] = new Dictionary<string, object> { { "booleanValue", b } };
                else if (kv.Value is DateTime dt)
                    firestoreFields[kv.Key] = new Dictionary<string, object> { { "timestampValue", dt.ToString("o") } };
                else
                    firestoreFields[kv.Key] = new Dictionary<string, object> { { "stringValue", kv.Value?.ToString() ?? string.Empty } };
            }
            var docPayload = new { fields = firestoreFields };
            var request = new HttpRequestMessage(HttpMethod.Patch, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(docPayload), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);
            var resp = await _httpClient.SendAsync(request);
            if (!resp.IsSuccessStatusCode)
            {
                var err = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Falha ao salvar usuário no Firestore: {err}");
            }
            return novoUsuario;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao registrar no Firebase: {ex.Message}");
        }
    }
}