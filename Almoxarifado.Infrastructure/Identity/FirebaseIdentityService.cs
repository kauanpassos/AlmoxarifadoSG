using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Almoxarifado.Domain.Interfaces;
using Almoxarifado.Domain.Enums;
using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;

namespace Almoxarifado.Infrastructure.Identity;

public sealed class FirebaseIdentityService : IIdentityService
{
    private readonly HttpClient _httpClient;
    private readonly FirestoreDb _firestoreDb;
    private readonly string _firebaseApiKey;

    public FirebaseIdentityService(
        HttpClient httpClient,
        IConfiguration configuration,
        FirestoreDb firestoreDb)
    {
        _httpClient = httpClient;
        _firestoreDb = firestoreDb;
        _firebaseApiKey = configuration["Firebase:ApiKey"]
            ?? throw new ArgumentNullException(
                nameof(configuration),
                "A ApiKey do Firebase não foi configurada.");
    }

    public async Task<string> AuthenticateAsync(string email, string password)
    {
        var url =
            $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_firebaseApiKey}";

        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                url,
                new
                {
                    email,
                    password,
                    returnSecureToken = true
                });

            if (!response.IsSuccessStatusCode)
            {
                throw new UnauthorizedAccessException(
                    "E-mail ou senha inválidos. Verifique suas credenciais.");
            }

            var authResult = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (authResult is null || string.IsNullOrWhiteSpace(authResult.IdToken))
            {
                throw new InvalidOperationException(
                    "Falha ao autenticar. O provedor não retornou um token válido.");
            }

            var userDocRef = _firestoreDb
                .Collection("Usuarios")
                .Document(authResult.LocalId);

            var snapshot = await userDocRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                var tipoValue = snapshot.ContainsField("Tipo")
                    ? snapshot.GetValue<long>("Tipo")
                    : 0;

                if (tipoValue == (long)TipoUsuario.Almoxarife)
                {
                    await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(
                        authResult.LocalId,
                        new Dictionary<string, object>
                        {
                            { "role", "Almoxarife" }
                        });

                    var refreshedResponse = await _httpClient.PostAsJsonAsync(
                        url,
                        new
                        {
                            email,
                            password,
                            returnSecureToken = true
                        });

                    if (refreshedResponse.IsSuccessStatusCode)
                    {
                        var refreshedResult =
                            await refreshedResponse.Content.ReadFromJsonAsync<AuthResponse>();

                        if (!string.IsNullOrWhiteSpace(refreshedResult?.IdToken))
                            return refreshedResult.IdToken;
                    }
                }
                else
                {
                    await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(
                        authResult.LocalId,
                        new Dictionary<string, object>
                        {
                            { "role", "Colaborador" }
                        });
                }
            }

            return authResult.IdToken;
        }
        catch (HttpRequestException)
        {
            throw new InvalidOperationException(
                "Não foi possível conectar ao provedor de identidade.");
        }
    }

    public async Task<string> CreateUserAsync(
        string email,
        string password,
        string displayName,
        string role)
    {
        var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(
            new UserRecordArgs
            {
                Email = email,
                Password = password,
                DisplayName = displayName,
                Disabled = false
            });

        await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(
            userRecord.Uid,
            new Dictionary<string, object>
            {
                { "role", role }
            });

        return userRecord.Uid;
    }

    public Task DeleteUserAsync(string uid)
        => FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);
}

public sealed record AuthResponse
{
    [JsonPropertyName("idToken")]
    public string IdToken { get; init; } = string.Empty;

    [JsonPropertyName("localId")]
    public string LocalId { get; init; } = string.Empty;
}