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

    public AuthService(FirebaseAuthClient firebaseAuthClient)
    {
        _firebaseAuthClient = firebaseAuthClient;
    }

    public async Task<Usuario?> LoginAsync(
        string email,
        string password
    )
    {
        try
        {
            var userCredential =
                await _firebaseAuthClient
                    .SignInWithEmailAndPasswordAsync(
                        email,
                        password
                    );

            if (userCredential?.User?.Credential is null)
                return null;

            var token =
                userCredential.User.Credential.IdToken;

            if (string.IsNullOrWhiteSpace(token))
                return null;

            await SecureStorage.Default.SetAsync(
                "auth_token",
                token
            );

            var tipo =
                ExtrairTipoUsuarioDoJwt(token);

            return new Usuario(
                id: userCredential.User.Uid,
                nome:
                    userCredential.User.Info.DisplayName
                    ?? email,
                email:
                    userCredential.User.Info.Email
                    ?? email,
                setor: "Não Informado",
                tipo: tipo
            );
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
            var token =
                await SecureStorage.Default
                    .GetAsync("auth_token");

            if (string.IsNullOrWhiteSpace(token))
                return null;

            var parts = token.Split('.');

            if (parts.Length < 2)
                return null;

            var payload = parts[1];

            payload = payload
                .PadRight(
                    payload.Length
                    + (4 - payload.Length % 4) % 4,
                    '='
                )
                .Replace('-', '+')
                .Replace('_', '/');

            var jsonBytes =
                Convert.FromBase64String(payload);

            using var jsonDoc =
                JsonDocument.Parse(jsonBytes);

            string id =
                jsonDoc.RootElement
                    .TryGetProperty(
                        "sub",
                        out var subProp
                    )
                ? subProp.GetString()
                    ?? string.Empty
                : string.Empty;

            string email =
                jsonDoc.RootElement
                    .TryGetProperty(
                        "email",
                        out var emailProp
                    )
                ? emailProp.GetString()
                    ?? string.Empty
                : string.Empty;

            string nome =
                jsonDoc.RootElement
                    .TryGetProperty(
                        "name",
                        out var nameProp
                    )
                ? nameProp.GetString()
                    ?? email
                : email;

            if (string.IsNullOrWhiteSpace(id))
                return null;

            var tipo =
                ExtrairTipoUsuarioDoJwt(token);

            return new Usuario(
                id: id,
                nome: nome,
                email: email,
                setor: "Não Informado",
                tipo: tipo
            );
        }
        catch
        {
            return null;
        }
    }

    public async Task LogoutAsync()
    {
        SecureStorage.Default.Remove(
            "auth_token"
        );

        _firebaseAuthClient.SignOut();

        await Task.CompletedTask;
    }

    private TipoUsuario ExtrairTipoUsuarioDoJwt(
        string jwt
    )
    {
        try
        {
            var parts = jwt.Split('.');

            if (parts.Length < 2)
                return TipoUsuario.Colaborador;

            var payload = parts[1];

            payload = payload
                .PadRight(
                    payload.Length
                    + (4 - payload.Length % 4) % 4,
                    '='
                )
                .Replace('-', '+')
                .Replace('_', '/');

            var jsonBytes =
                Convert.FromBase64String(payload);

            using var jsonDoc =
                JsonDocument.Parse(jsonBytes);

            if (
                jsonDoc.RootElement
                    .TryGetProperty(
                        "role",
                        out var roleElement
                    )
            )
            {
                var role =
                    roleElement.GetString();

                if (
                    Enum.TryParse<TipoUsuario>(
                        role,
                        true,
                        out var tipo
                    )
                )
                {
                    return tipo;
                }
            }
        }
        catch
        {
        }

        return TipoUsuario.Colaborador;
    }
}