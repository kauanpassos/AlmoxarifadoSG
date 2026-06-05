using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Almoxarifado.App.Models;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.Domain.Entities;
using Almoxarifado.Domain.Enums;
using Firebase.Auth;
using Microsoft.Maui.Storage;

namespace Almoxarifado.App.Services;

public sealed class AuthService(FirebaseAuthClient firebaseAuthClient, HttpClient httpClient) : IAuthService
{
    public async Task<Usuario?> LoginAsync(string email, string password)
    {
        return await ExecutarRequisicaoSeguraAsync(async () =>
        {
            var response = await httpClient.PostAsJsonAsync("api/auth/login", new { email, password });

            if (!response.IsSuccessStatusCode)
                throw new UnauthorizedAccessException("E-mail ou senha inválidos. Verifique suas credenciais.");

            using var jsonDocResponse = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
            if (!jsonDocResponse.RootElement.TryGetProperty("token", out var tokenProperty))
                throw new InvalidOperationException("Falha ao obter o token de acesso. Servidor retornou um formato inválido.");

            var token = tokenProperty.GetString();
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("O token recebido do servidor está vazio.");

            await SecureStorage.Default.SetAsync("auth_token", token);

            return await FetchUserProfileAsync(token, email);
        });
    }

    public async Task RegistrarAsync(RegistrarUsuarioRequest request)
    {
        await ExecutarRequisicaoSeguraAsync(async () =>
        {
            var payload = new { nome = request.Nome, email = request.Email, senha = request.Senha, setor = request.Setor, tipo = (int)request.Tipo };
            var response = await httpClient.PostAsJsonAsync("api/auth/registrar", payload);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Falha no cadastro: {errorContent}");
            }

            return true;
        });
    }

    private async Task<T> ExecutarRequisicaoSeguraAsync<T>(Func<Task<T>> acao)
    {
        try
        {
            return await acao();
        }
        catch (HttpRequestException)
        {
            throw new InvalidOperationException("Não foi possível conectar ao servidor. Verifique sua conexão com a internet.");
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new InvalidOperationException("Erro inesperado de comunicação. Tente novamente mais tarde.");
        }
    }

    public async Task<Usuario?> VerificarSessaoAtivaAsync()
    {
        try
        {
            var token = await SecureStorage.Default.GetAsync("auth_token");
            return string.IsNullOrWhiteSpace(token) ? default : ExtractUserFromJwt(token);
        }
        catch
        {
            return default;
        }
    }

    public Task LogoutAsync()
    {
        try
        {
            if (firebaseAuthClient.User != null)
            {
                firebaseAuthClient.SignOut();
            }
        }
        catch
        { }
        finally
        {
            SecureStorage.Default.Remove("auth_token");
        }

        return Task.CompletedTask;
    }

    private async Task<Usuario> FetchUserProfileAsync(string token, string defaultEmail)
    {
        var baseUser = ExtractUserFromJwt(token) ?? new Usuario(string.Empty, defaultEmail, defaultEmail, "Não Informado", TipoUsuario.Colaborador);

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/Auth/perfil");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return baseUser;

            var content = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(content);
            var root = jsonDoc.RootElement;

            string GetStringFallback(string key1, string key2, string fallback) =>
                root.TryGetProperty(key1, out var p1) ? p1.GetString() ?? fallback :
                root.TryGetProperty(key2, out var p2) ? p2.GetString() ?? fallback : fallback;

            var role = baseUser.Tipo;
            if (root.TryGetProperty("tipo", out var roleProp) || root.TryGetProperty("Tipo", out roleProp))
            {
                if (roleProp.ValueKind == JsonValueKind.Number)
                {
                    role = (TipoUsuario)roleProp.GetInt32();
                }
                else if (roleProp.ValueKind == JsonValueKind.String)
                {
                    if (Enum.TryParse<TipoUsuario>(roleProp.GetString(), true, out var parsedRole))
                    {
                        role = parsedRole;
                    }
                }
            }

            return new Usuario(
                id: baseUser.Id,
                nome: GetStringFallback("nome", "Nome", baseUser.Nome),
                email: baseUser.Email,
                setor: GetStringFallback("setor", "Setor", baseUser.Setor),
                tipo: role);
        }
        catch
        {
            return baseUser;
        }
    }

    private Usuario? ExtractUserFromJwt(string token)
    {
        try
        {
            using var jsonDoc = ParseJwtPayload(token);
            if (jsonDoc is null)
                return default;

            var root = jsonDoc.RootElement;

            var id = root.TryGetProperty("sub", out var subProp) ? subProp.GetString() : string.Empty;
            if (string.IsNullOrWhiteSpace(id))
                return default;

            var email = root.TryGetProperty("email", out var emailProp) ? emailProp.GetString() ?? string.Empty : string.Empty;
            var name = root.TryGetProperty("name", out var nameProp) ? nameProp.GetString() ?? email : email;

            var role = TipoUsuario.Colaborador;
            if (root.TryGetProperty("role", out var roleProp) || root.TryGetProperty("Role", out roleProp))
            {
                if (string.Equals(roleProp.GetString(), "Almoxarife", StringComparison.OrdinalIgnoreCase))
                    role = TipoUsuario.Almoxarife;
            }

            return new Usuario(id, name, email, "Não Informado", role);
        }
        catch
        {
            return default;
        }
    }

    private JsonDocument? ParseJwtPayload(string token)
    {
        var parts = token.Split('.');
        if (parts.Length < 2)
            return default;

        var payload = parts[1].PadRight(parts[1].Length + (4 - parts[1].Length % 4) % 4, '=').Replace('-', '+').Replace('_', '/');
        return JsonDocument.Parse(Convert.FromBase64String(payload));
    }
}