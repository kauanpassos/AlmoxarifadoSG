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

public class FirebaseIdentityService : IIdentityService
{
    private readonly HttpClient _httpClient;
    private readonly string _firebaseApiKey;
    public FirebaseIdentityService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _firebaseApiKey = configuration["Firebase:ApiKey"]
            ?? throw new ArgumentNullException(nameof(configuration), "A ApiKey do Firebase não foi configurada no appsettings.json da API.");
    }
    public async Task<string?> AutenticarAsync(string email, string senha)
    {
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_firebaseApiKey}";

        var requestBody = new
        {
            email = email,
            password = senha,
            returnSecureToken = true
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var erroReal = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"\n=== ERRO NO FIREBASE ===\n{erroReal}\n========================\n");

                return null;
            }

            var authResult = await response.Content.ReadFromJsonAsync<AuthResponse>();

            return authResult?.IdToken;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n=== EXCEÇÃO NO LOGIN ===\n{ex.Message}\n========================\n");
            return null;
        }
    }

    public async Task<string> CreateUserAsync(string email, string password, string displayName, string role)
    {
        var args = new UserRecordArgs()
        {
            Email = email,
            Password = password,
            DisplayName = displayName,
            Disabled = false
        };

        var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(args);

        var claims = new Dictionary<string, object>
        {
            { "role", role }
        };

        await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userRecord.Uid, claims);

        return userRecord.Uid;
    }

    public async Task DeleteUserAsync(string uid)
    {
        await FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);
    }
}
public class AuthResponse
{
    [JsonPropertyName("idToken")]
    public string IdToken { get; set; } = string.Empty;
}