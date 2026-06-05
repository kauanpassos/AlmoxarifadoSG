using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Almoxarifado.Domain.Interfaces;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Configuration;

namespace Almoxarifado.Infrastructure.Identity;

public sealed class FirebaseIdentityService : IIdentityService
{
    private readonly HttpClient _httpClient;
    private readonly string _firebaseApiKey;

    public FirebaseIdentityService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _firebaseApiKey = configuration["Firebase:ApiKey"]
            ?? throw new ArgumentNullException(nameof(configuration), "A ApiKey do Firebase não foi configurada no appsettings.json da API.");
    }

    public async Task<string> AuthenticateAsync(string email, string password)
    {
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_firebaseApiKey}";

        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, new { email, password, returnSecureToken = true });

            if (!response.IsSuccessStatusCode)
                throw new UnauthorizedAccessException("E-mail ou senha inválidos. Verifique suas credenciais.");

            var authResult = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (authResult is null || string.IsNullOrWhiteSpace(authResult.IdToken))
                throw new InvalidOperationException("Falha ao autenticar. O provedor de identidade não retornou um token válido.");

            return authResult.IdToken;
        }
        catch (HttpRequestException)
        {
            throw new InvalidOperationException("Não foi possível conectar ao provedor de identidade.");
        }
    }

    public async Task<string> CreateUserAsync(string email, string password, string displayName, string role)
    {
        var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs
        {
            Email = email,
            Password = password,
            DisplayName = displayName,
            Disabled = false
        });

        await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userRecord.Uid, new Dictionary<string, object> { { "role", role } });

        return userRecord.Uid;
    }

    public Task DeleteUserAsync(string uid) => FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);
}

public sealed record AuthResponse
{
    [JsonPropertyName("idToken")]
    public string IdToken { get; init; } = string.Empty;
}