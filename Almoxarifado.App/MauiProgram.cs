using Almoxarifado.App.Services;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.App.ViewModels;
using Almoxarifado.App.Views;
using Almoxarifado.Domain.Entities;
using Almoxarifado.Domain.Interfaces;
using CommunityToolkit.Maui;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Microsoft.Extensions.Logging;

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

        var config = new FirebaseAuthConfig
        {
            ApiKey = "AIzaSyCD1hywuoaY2Bmls2sz5hXzJG-KyruJVYE",
            AuthDomain = "almoxarifado-sg.firebaseapp.com",
            Providers = new FirebaseAuthProvider[]
            {
                new EmailProvider()
            }
        };

        builder.Services.AddSingleton(new FirebaseAuthClient(config));
        string apiBaseUrl = DeviceInfo.Platform == DevicePlatform.Android
            ? "https://10.0.2.2:7185/"
            : "https://localhost:7185/";

        builder.Services.AddTransient<AuthenticatedHttpHandler>();

        builder.Services.AddScoped(sp =>
        {
            var authHandler = sp.GetRequiredService<AuthenticatedHttpHandler>();

            var innerHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            authHandler.InnerHandler = innerHandler;

            return new HttpClient(authHandler)
            {
                BaseAddress = new Uri(apiBaseUrl)
            };
        });

        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<INavigationService, NavigationService>();

        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<EstoqueViewModel>();
        builder.Services.AddTransient<GestaoFilaViewModel>();
        builder.Services.AddTransient<HomeColaboradorViewModel>();
        builder.Services.AddTransient<NovaSolicitacaoViewModel>();
        builder.Services.AddTransient<PerfilViewModel>();

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<GestaoFilaPage>();
        builder.Services.AddTransient<EstoquePage>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<HomeColaboradorPage>();
        builder.Services.AddTransient<NovaSolicitacaoPage>();
        builder.Services.AddTransient<PerfilPage>();

        return builder.Build();
    }
}