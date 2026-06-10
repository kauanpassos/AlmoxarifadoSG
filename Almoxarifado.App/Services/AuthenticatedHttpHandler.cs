using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace Almoxarifado.App.Services;

public class AuthenticatedHttpHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            var token = await SecureStorage.Default.GetAsync("auth_token");

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TOKEN ERROR] Falha ao injetar token: {ex.Message}");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}