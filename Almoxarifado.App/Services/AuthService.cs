using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Almoxarifado.App.Models;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.Application.DTOs;
using Almoxarifado.Domain.Enums;
using Firebase.Auth;
using Microsoft.Maui.Storage;

namespace Almoxarifado.App.Services;

public sealed class AuthService(FirebaseAuthClient firebaseAuthClient, HttpClient httpClient) : IAuthService
{
    private const string AuthTokenKey = "auth_token";
    private const string ApiAuthLogin = "api/auth/login";
    private const string ApiAuthRegistrar = "api/auth/registrar";
    private const string ApiAuthPerfil = "api/Auth/perfil";
    private const string DefaultNotInformed = "Não Informado";

    public async Task<UsuarioDto?> LoginAsync(string email, string password)
    {
        return await ExecutarRequisicaoSeguraAsync(async () =>
        {
            var response = await httpClient.PostAsJsonAsync(ApiAuthLogin, new { email, password });

            if (response.IsSuccessStatusCode is false)
                throw new UnauthorizedAccessException("E-mail ou senha inválidos. Verifique as suas credenciais.");

            using var jsonDocResponse = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
            
            if (!jsonDocResponse.RootElement.TryGetProperty("token", out var tokenProperty))
                throw new InvalidOperationException("Falha ao obter o token de acesso. O formato retornado pelo servidor é inválido.");

            var token = tokenProperty.GetString();
            
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("O token recebido do servidor está vazio.");

            await SecureStorage.Default.SetAsync(AuthTokenKey, token);

            var usuario = await FetchUserProfileAsync(token, email);

            if (usuario is not null)
                UsuarioSessao.UsuarioLogado = usuario;

            return usuario;
        });
    }

    public async Task RegistrarAsync(RegistrarUsuarioRequest request)
    {
        await ExecutarRequisicaoSeguraAsync(async () =>
        {
            var payload = new { nome = request.Nome, email = request.Email, senha = request.Senha, setor = request.Setor, tipo = (int)request.Tipo };
            var response = await httpClient.PostAsJsonAsync(ApiAuthRegistrar, payload);

            if (response.IsSuccessStatusCode is false)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Falha no registo: {errorContent}");
            }

            return true;
        });
    }

    public async Task<UsuarioDto?> VerificarSessaoAtivaAsync()
    {
        try
        {
            var token = await SecureStorage.Default.GetAsync(AuthTokenKey);
            if (string.IsNullOrWhiteSpace(token)) 
                return default;

            var usuario = ExtractUserFromJwt(token);
            
            if (usuario is not null)
                UsuarioSessao.UsuarioLogado = usuario;
                
            return usuario;
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
            if (firebaseAuthClient.User is not null)
                firebaseAuthClient.SignOut();
        }
        catch
        { }
        finally
        {
            SecureStorage.Default.Remove(AuthTokenKey);
            UsuarioSessao.UsuarioLogado = default;
        }

        return Task.CompletedTask;
    }

    public string? ObterUsuarioIdAtual() => UsuarioSessao.UsuarioLogado?.Id;

    private async Task<UsuarioDto> FetchUserProfileAsync(string token, string defaultEmail)
    {
        var baseUser = ExtractUserFromJwt(token) ?? new UsuarioDto(string.Empty, defaultEmail, defaultEmail, DefaultNotInformed, TipoUsuario.Colaborador);

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, ApiAuthPerfil);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.SendAsync(request);
            
            if (response.IsSuccessStatusCode is false)
                return baseUser;

            var content = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(content);
            var root = jsonDoc.RootElement;

            return new UsuarioDto(
                ObterValorJson(root, baseUser.Id, "id", "Id"),
                baseUser.Email,
                ObterValorJson(root, baseUser.Nome, "nome", "Nome"),
                ObterValorJson(root, baseUser.Setor, "setor", "Setor"),
                ExtrairTipoUsuario(root, baseUser.Tipo)
            );
        }
        catch
        {
            return baseUser;
        }
    }

    private UsuarioDto? ExtractUserFromJwt(string token)
    {
        try
        {
            using var jsonDoc = ParseJwtPayload(token);
            if (jsonDoc is null)
                return default;

            var root = jsonDoc.RootElement;

            var id = ObterValorJson(root, string.Empty, "user_id", "sub");
            if (string.IsNullOrWhiteSpace(id))
                return default;

            var email = ObterValorJson(root, string.Empty, "email");
            var name = ObterValorJson(root, email, "name");
            var role = ObterRoleDoJwt(root);

            return new UsuarioDto(id, email, name, DefaultNotInformed, role);
        }
        catch
        {
            return default;
        }
    }

    private static string ObterValorJson(JsonElement root, string fallback, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (root.TryGetProperty(key, out var prop) && prop.ValueKind is not JsonValueKind.Null)
            {
                var value = prop.GetString();
                if (!string.IsNullOrWhiteSpace(value))
                    return value;
            }
        }
        return fallback;
    }

    private static TipoUsuario ExtrairTipoUsuario(JsonElement root, TipoUsuario fallback)
    {
        foreach (var key in new[] { "tipo", "Tipo" })
        {
            if (root.TryGetProperty(key, out var prop))
            {
                if (prop.ValueKind is JsonValueKind.Number)
                    return (TipoUsuario)prop.GetInt32();
                    
                if (prop.ValueKind is JsonValueKind.String && Enum.TryParse<TipoUsuario>(prop.GetString(), true, out var parsedRole))
                    return parsedRole;
            }
        }
        return fallback;
    }

    private static TipoUsuario ObterRoleDoJwt(JsonElement root)
    {
        foreach (var key in new[] { "role", "Role" })
        {
            if (root.TryGetProperty(key, out var prop) && 
                string.Equals(prop.GetString(), "Almoxarife", StringComparison.OrdinalIgnoreCase))
            {
                return TipoUsuario.Almoxarife;
            }
        }
        return TipoUsuario.Colaborador;
    }

    private static JsonDocument? ParseJwtPayload(string token)
    {
        var parts = token.Split('.');
        if (parts.Length < 2)
            return default;

        var payload = parts[1]
            .PadRight(parts[1].Length + (4 - parts[1].Length % 4) % 4, '=')
            .Replace('-', '+')
            .Replace('_', '/');

        return JsonDocument.Parse(Convert.FromBase64String(payload));
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
        catch (Exception ex) when (ex is not UnauthorizedAccessException && ex is not InvalidOperationException)
        {
            throw new InvalidOperationException("Ocorreu um erro inesperado de comunicação. Por favor, tente novamente mais tarde.");
        }
    }
}