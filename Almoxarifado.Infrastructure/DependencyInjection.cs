using System;
using System.IO;
using Almoxarifado.Domain.Interfaces;
using Almoxarifado.Infrastructure.Identity;
using Almoxarifado.Infrastructure.Repositories;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Almoxarifado.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
        services.AddScoped<EstoqueRepository>();
        services.AddScoped(typeof(IReadOnlyRepository<>), typeof(FirebaseRepository<>));
        services.AddScoped(typeof(IWriteOnlyRepository<>), typeof(FirebaseRepository<>));

        services.AddScoped<IIdentityService, FirebaseIdentityService>();

        var firebaseKeyPath = configuration["Firebase:CredentialsPath"];

        if (string.IsNullOrWhiteSpace(firebaseKeyPath))
            throw new ArgumentNullException(nameof(configuration), "O caminho das credenciais do Firebase Admin não foi configurado.");

        string fullPath = Path.Combine(AppContext.BaseDirectory, firebaseKeyPath);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Arquivo de credenciais do Firebase não encontrado: {fullPath}");

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Testing")
        {
            if (FirebaseApp.DefaultInstance is null)
            {
                var credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(fullPath);

                FirebaseApp.Create(new AppOptions
                {
                    Credential = credential
                });
            }
        }

        return services;
    }
}