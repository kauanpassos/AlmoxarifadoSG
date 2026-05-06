using Almoxarifado.App.Services;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.App.ViewModels;
using Almoxarifado.App.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Firebase.Auth;
using Firebase.Auth.Providers;

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
        var config = new FirebaseAuthConfig
        {
            ApiKey = "AIzaSyCD1hywuoaY2Bmls2sz5hXzJG-KyruJVYE",
            AuthDomain = "almoxarifado-sg.firebaseapp.com",
            Providers = new FirebaseAuthProvider[]
            {
                new EmailProvider()
            }
        };

        var client = new FirebaseAuthClient(config);
        builder.Services.AddSingleton(client);

        builder.Services.AddSingleton(new HttpClient());

        var projectId = "almoxarifado-sg";
        var apiKey = "AIzaSyCD1hywuoaY2Bmls2sz5hXzJG-KyruJVYE";

        builder.Services.AddSingleton(projectId);
        builder.Services.AddSingleton(apiKey);

        builder.Services.AddSingleton<IAuthService>(sp => new AuthService(sp.GetRequiredService<FirebaseAuthClient>(), sp.GetRequiredService<HttpClient>(), projectId, apiKey));

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<IFirebaseService>(sp => new FirebaseService(sp.GetRequiredService<HttpClient>(), projectId, apiKey));

        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<GestaoFilaViewModel>();

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<GestaoFilaPage>();
        builder.Services.AddTransient<EstoquePage>();
        builder.Services.AddTransient<MainPage>();

        return builder.Build();
    }
}