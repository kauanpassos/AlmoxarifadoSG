using Almoxarifado.App.Services;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.App.ViewModels;
using Almoxarifado.App.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Firebase.Database;
using Almoxarifado.Domain;
using Firebase.Auth;

namespace Almoxarifado.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit() // Agora reconhecido com o pacote instalado
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Configuração Firebase
        var firebaseUrl = "URL_DO_FIREBASE_AQUI";
        var firebaseApiKey = "API_KEY_AQUI";
        var projectId = "PROJECT_ID_AQUI";

        var firebaseClient = new FirebaseClient(firebaseUrl);
        builder.Services.AddSingleton(firebaseClient);

        // Configuração do FirebaseAuthClient
        var authClient = new FirebaseAuthClient(new FirebaseAuthConfigs
        {
            ApiKey = firebaseApiKey,
            AuthDomain = $"{projectId}.firebaseapp.com",
            Providers = new[] { new EmailPasswordAuthProvider() }
        });
        builder.Services.AddSingleton(authClient);
        
        builder.Services.AddSingleton<HttpClient>();

        // Infra
        builder.Services.AddScoped<IEngine<Estoque>>(sp => new FirebaseEngine<Estoque>(firebaseClient, "estoque"));
        builder.Services.AddScoped<IEngine<Solicitacao>>(sp => new FirebaseEngine<Solicitacao>(firebaseClient, "solicitacoes"));

        // Repositórios
        builder.Services.AddScoped<Almoxarifado.Domain.Interfaces.IReadOnlyRepository<Estoque>>(sp => sp.GetRequiredService<IEngine<Estoque>>());
        builder.Services.AddScoped<Almoxarifado.Domain.Interfaces.IWriteOnlyRepository<Estoque>>(sp => sp.GetRequiredService<IEngine<Estoque>>());
        builder.Services.AddScoped<Almoxarifado.Domain.Interfaces.IReadOnlyRepository<Solicitacao>>(sp => sp.GetRequiredService<IEngine<Solicitacao>>());
        builder.Services.AddScoped<Almoxarifado.Domain.Interfaces.IWriteOnlyRepository<Solicitacao>>(sp => sp.GetRequiredService<IEngine<Solicitacao>>());

        // Serviços
        builder.Services.AddScoped<IAuthService>(sp => new AuthService(
            sp.GetRequiredService<FirebaseAuthClient>(),
            sp.GetRequiredService<HttpClient>(),
            projectId,
            firebaseApiKey));

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<EstoqueViewModel>();
        builder.Services.AddTransient<GestaoFilaViewModel>();

        // Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<GestaoFilaPage>();
        builder.Services.AddTransient<EstoquePage>();
        builder.Services.AddTransient<MainPage>();

        return builder.Build();
    }
}