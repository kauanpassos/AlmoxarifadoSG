using System;
using System.IO;
using Almoxarifado.Domain.Interfaces;
using Almoxarifado.Infrastructure.Identity;
using Almoxarifado.Infrastructure.Repositories;
using FirebaseAdmin;
using Firebase.Database;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Almoxarifado.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();

        var firebaseUrl = configuration["Firebase:Url"];
        if (string.IsNullOrWhiteSpace(firebaseUrl))
            throw new ArgumentNullException(nameof(configuration), "A URL do Firebase Realtime Database não foi configurada.");

        services.AddSingleton(new FirebaseClient(firebaseUrl));

        services.AddScoped<EstoqueRepository>();

        services.AddScoped(typeof(IReadOnlyRepository<>), typeof(FirebaseRepository<>));
        services.AddScoped(typeof(IWriteOnlyRepository<>), typeof(FirebaseRepository<>));

        services.AddScoped<IIdentityService, FirebaseIdentityService>();

        var firebaseKeyPath = configuration["Firebase:CredentialsPath"];

        if (string.IsNullOrWhiteSpace(firebaseKeyPath))
            throw new ArgumentNullException(nameof(configuration), "O caminho das credenciais do Firebase Admin não foi configurado.");

        if (!File.Exists(firebaseKeyPath))
            throw new FileNotFoundException($"Arquivo de credenciais do Firebase não encontrado: {firebaseKeyPath}");

        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(firebaseKeyPath)
            });
        }

        return services;
    }
}