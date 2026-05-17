using Almoxarifado.App.Services;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.App.ViewModels;
using Almoxarifado.App.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Firebase.Database;
using Almoxarifado.Domain;
using Almoxarifado.Domain.Interfaces;
using Firebase.Auth;
using Firebase.Auth.Providers;
using MediatR;
using System.Net.Http;

namespace Almoxarifado.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // --- CONFIGURAÇÃO FIREBASE ---

        var firebaseUrl = "https://almoxarifado-sg-default-rtdb.firebaseio.com/";
        var firebaseApiKey = "AIzaSyCD1hywuoaY2Bmls2sz5hXzJG-KyruJVYE";
        var projectId = "almoxarifado-sg";

        // Firebase Realtime Database
        var firebaseClient = new FirebaseClient(firebaseUrl);
        builder.Services.AddSingleton(firebaseClient);

        // Configuração do FirebaseAuthClient
        var config = new FirebaseAuthConfig
        {
            ApiKey = firebaseApiKey,
            AuthDomain = $"{projectId}.firebaseapp.com",
            Providers = new FirebaseAuthProvider[]
            {
                new EmailProvider()
            }
        };

        var authClient = new FirebaseAuthClient(config);
        builder.Services.AddSingleton(authClient);
        builder.Services.AddSingleton<HttpClient>();

        // Infra e Repositórios
        builder.Services.AddScoped<IEngine<Estoque>>(sp => new FirebaseEngine<Estoque>(firebaseClient, "estoque"));
        builder.Services.AddScoped<IEngine<Solicitacao>>(sp => new FirebaseEngine<Solicitacao>(firebaseClient, "solicitacoes"));

        builder.Services.AddScoped<IReadOnlyRepository<Estoque>>(sp => sp.GetRequiredService<IEngine<Estoque>>());
        builder.Services.AddScoped<IWriteOnlyRepository<Estoque>>(sp => sp.GetRequiredService<IEngine<Estoque>>());
        builder.Services.AddScoped<IReadOnlyRepository<Solicitacao>>(sp => sp.GetRequiredService<IEngine<Solicitacao>>());
        builder.Services.AddScoped<IWriteOnlyRepository<Solicitacao>>(sp => sp.GetRequiredService<IEngine<Solicitacao>>());

        // Serviços
        builder.Services.AddScoped<IAuthService>(sp => new AuthService(
            sp.GetRequiredService<FirebaseAuthClient>(),
            sp.GetRequiredService<HttpClient>(),
            projectId,
            firebaseApiKey));

        builder.Services.AddScoped<INavigationService>(sp => new NavigationService(
            sp.GetRequiredService<IAuthService>()));

        // --- REGISTRO DO MEDIATR ---
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(MauiProgram).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(Almoxarifado.Application.Queries.GetEstoqueQuery).Assembly);
        });

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<EstoqueViewModel>();
        builder.Services.AddTransient<GestaoFilaViewModel>();
        builder.Services.AddTransient<HomeColaboradorViewModel>();
        builder.Services.AddTransient<NovaSolicitacaoViewModel>();
        builder.Services.AddTransient<PerfilViewModel>();
        builder.Services.AddTransient<Almoxarifado.App.ViewModels.PerfilViewModel>();
        
        // Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<GestaoFilaPage>();
        builder.Services.AddTransient<EstoquePage>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<HomeColaboradorPage>();
        builder.Services.AddTransient<NovaSolicitacaoPage>();
        builder.Services.AddTransient<PerfilPage>();
        builder.Services.AddTransient<Almoxarifado.App.Views.PerfilPage>();

        return builder.Build();
    }
}