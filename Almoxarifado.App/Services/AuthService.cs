using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.Domain;
using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Almoxarifado.App.Services
{
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

                var signInBody = new
                {
                    email,
                    password,
                    returnSecureToken = true
                };

                var payload = new StringContent(JsonSerializer.Serialize(signInBody), Encoding.UTF8, "application/json");
                var signInResponse = await _httpClient.PostAsync(signInUrl, payload);

                if (!signInResponse.IsSuccessStatusCode)
                {
                    var err = await signInResponse.Content.ReadAsStringAsync();
                    throw new Exception($"Falha ao autenticar: {err}");
                }

                var signInJson = await signInResponse.Content.ReadAsStringAsync();
                using var signInDoc = JsonDocument.Parse(signInJson);
                var root = signInDoc.RootElement;

                var idToken = root.GetProperty("idToken").GetString();
                var localId = root.GetProperty("localId").GetString();
                var userEmail = root.TryGetProperty("email", out var emailEl) ? emailEl.GetString() : email;

                if (string.IsNullOrEmpty(idToken) || string.IsNullOrEmpty(localId))
                    return null;

                var docName = $"projects/{_projectId}/databases/(default)/documents/Usuarios/{localId}";
                var url = $"https://firestore.googleapis.com/v1/{docName}";

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

                var resp = await _httpClient.SendAsync(request);

                if (!resp.IsSuccessStatusCode)
                {
                    return null;
                }

                var json = await resp.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                if (!doc.RootElement.TryGetProperty("fields", out var fields))
                    return null;

                string GetStringField(string name)
                    => fields.TryGetProperty(name, out var v) && v.TryGetProperty("stringValue", out var s) ? s.GetString() ?? string.Empty : string.Empty;

                return new Usuario(
                    id: localId,
                    nome: GetStringField("Nome"),
                    email: userEmail ?? string.Empty,
                    setor: GetStringField("Setor"),
                    tipo: GetStringField("Tipo")
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"O Firebase disse: {ex.Message}");
            }
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
                var signInBody = new
                {
                    email,
                    password = senha,
                    returnSecureToken = true
                };
                var payload = new StringContent(JsonSerializer.Serialize(signInBody), Encoding.UTF8, "application/json");
                var signInResponse = await _httpClient.PostAsync(signInUrl, payload);
                if (!signInResponse.IsSuccessStatusCode)
                {
                    var err = await signInResponse.Content.ReadAsStringAsync();
                    throw new Exception($"Falha ao autenticar após criação do usuário: {err}");
                }

                var signInJson = await signInResponse.Content.ReadAsStringAsync();
                using var signInDoc = JsonDocument.Parse(signInJson);
                var idToken = signInDoc.RootElement.GetProperty("idToken").GetString();

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

                var docPayload = new
                {
                    fields = firestoreFields
                };

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
}