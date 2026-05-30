using System.Net.Http.Headers;
using System.Text.Json;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.Domain.Entities;
using Almoxarifado.Domain.Enums;
using Firebase.Auth;
using Microsoft.Maui.Storage;

namespace Almoxarifado.App.Services;

public class AuthService : IAuthService
{
    private readonly FirebaseAuthClient _firebaseAuthClient;
    private readonly HttpClient _httpClient;

    public AuthService(FirebaseAuthClient firebaseAuthClient, HttpClient httpClient)
    {
        _firebaseAuthClient = firebaseAuthClient;
        _httpClient = httpClient;
    }

    public async Task<Usuario?> LoginAsync(string email, string password)
    {
        try
        {
            var userCredential = await _firebaseAuthClient.SignInWithEmailAndPasswordAsync(email, password);

            if (userCredential?.User?.Credential is null) return null;

            var token = userCredential.User.Credential.IdToken;

            if (string.IsNullOrWhiteSpace(token)) return null;

            await SecureStorage.Default.SetAsync("auth_token", token);

            return await BuscarPerfilNaApiAsync(token, userCredential.User.Uid, email);
        }
        catch
        {
            return null;
        }
    }

    public async Task<Usuario?> VerificarSessaoAtivaAsync()
    {
        try
        {
            var token = await SecureStorage.Default.GetAsync("auth_token");

            if (string.IsNullOrWhiteSpace(token)) return null;

            var parts = token.Split('.');
            if (parts.Length < 2) return null;

            var payload = parts[1].PadRight(parts[1].Length + (4 - parts[1].Length % 4) % 4, '=').Replace('-', '+').Replace('_', '/');
            using var jsonDoc = JsonDocument.Parse(Convert.FromBase64String(payload));

            string id = jsonDoc.RootElement.TryGetProperty("sub", out var subProp) ? subProp.GetString() ?? string.Empty : string.Empty;
            string email = jsonDoc.RootElement.TryGetProperty("email", out var emailProp) ? emailProp.GetString() ?? string.Empty : string.Empty;

            if (string.IsNullOrWhiteSpace(id)) return null;

            return await BuscarPerfilNaApiAsync(token, id, email);
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
            _firebaseAuthClient.SignOut();
        }
        catch (NullReferenceException) { }
        catch (Exception ex)
        {
            Console.WriteLine($"Aviso ao deslogar do Firebase: {ex.Message}");
        }
        finally
        {
            SecureStorage.Default.Remove("auth_token");
        }
        await Task.CompletedTask;
    }

    private async Task<Usuario> BuscarPerfilNaApiAsync(string token, string fallbackId, string fallbackEmail)
    {
        var tipoUsuario = TipoUsuario.Colaborador;
        string nome = fallbackEmail;
        string setor = "Não Informado";

        try
        {
            var parts = token.Split('.');
            if (parts.Length >= 2)
            {
                var payload = parts[1].PadRight(parts[1].Length + (4 - parts[1].Length % 4) % 4, '=').Replace('-', '+').Replace('_', '/');
                using var payloadDoc = JsonDocument.Parse(Convert.FromBase64String(payload));

                if (payloadDoc.RootElement.TryGetProperty("role", out var roleProp) ||
                    payloadDoc.RootElement.TryGetProperty("Role", out roleProp))
                {
                    var roleString = roleProp.GetString();
                    if (roleString == "Almoxarife")
                    {
                        tipoUsuario = TipoUsuario.Almoxarife;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao extrair role do token: {ex.Message}");
        }

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/Auth/perfil");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(content);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("nome", out var n) || root.TryGetProperty("Nome", out n))
                    nome = n.GetString() ?? fallbackEmail;

                if (root.TryGetProperty("setor", out var s) || root.TryGetProperty("Setor", out s))
                    setor = s.GetString() ?? "Não Informado";

                if (root.TryGetProperty("tipo", out var t) || root.TryGetProperty("Tipo", out t))
                {
                    tipoUsuario = (TipoUsuario)t.GetInt32();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao consultar perfil na API: {ex.Message}");
        }

        return new Usuario(id: fallbackId, nome: nome, email: fallbackEmail, setor: setor, tipo: tipoUsuario);
    }
}