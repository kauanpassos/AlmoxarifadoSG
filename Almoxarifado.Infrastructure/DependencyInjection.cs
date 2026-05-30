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

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Testing")
        {
            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"Arquivo de credenciais do Firebase não encontrado: {fullPath}");

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", fullPath);

            if (FirebaseApp.DefaultInstance is null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.GetApplicationDefault()
                });
            }
        }

        services.AddSingleton(sp => 
        {
            var projectId = configuration["Firebase:ProjectId"] ?? "almoxarifado-sg";
            return Google.Cloud.Firestore.FirestoreDb.Create(projectId);
        });

        return services;
    }
}