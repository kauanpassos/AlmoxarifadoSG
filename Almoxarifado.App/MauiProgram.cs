using Almoxarifado.App.Services;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.App.ViewModels;
using Almoxarifado.App.Views;
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

#if WINDOWS
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoBorder", (h, v) =>
        {
            h.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
            h.PlatformView.Background = null;
        });
        Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("NoBorder", (h, v) =>
        {
            h.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
            h.PlatformView.Background = null;
        });
#endif

#if DEBUG
        builder.Logging.AddDebug();
#endif

        RegistrarFirebase(builder.Services);
        RegistrarHttpClients(builder.Services);
        RegistrarServicos(builder.Services);
        RegistrarViewModels(builder.Services);
        RegistrarPaginas(builder.Services);

        return builder.Build();
    }

    private static void RegistrarFirebase(IServiceCollection services)
    {
        var config = new FirebaseAuthConfig
        {
            ApiKey = "AIzaSyCD1hywuoaY2Bmls2sz5hXzJG-KyruJVYE",
            AuthDomain = "almoxarifado-sg.firebaseapp.com",
            Providers = [new EmailProvider()]
        };

        services.AddSingleton(new FirebaseAuthClient(config));
    }

    private static void RegistrarHttpClients(IServiceCollection services)
    {
        // CORREÇÃO DEFINITIVA: 
        // Usando 127.0.0.1 em vez de localhost para furar o isolamento de rede do Windows App
        var apiBaseUrl = DeviceInfo.Platform == DevicePlatform.Android
            ? "http://10.0.2.2:5144/"
            : "http://127.0.0.1:5144/";

        services.AddTransient<AuthenticatedHttpHandler>();

        services.AddScoped(sp =>
        {
            var authHandler = sp.GetRequiredService<AuthenticatedHttpHandler>();
            authHandler.InnerHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            return new HttpClient(authHandler) { BaseAddress = new Uri(apiBaseUrl) };
        });
    }

    private static void RegistrarServicos(IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<INavigationService, NavigationService>();

        // Serviços vitais de Estoque
        services.AddScoped<IFirebaseService, HttpFirebaseService>();
        services.AddSingleton<ICartService, CartService>();
    }

    private static void RegistrarViewModels(IServiceCollection services)
    {
        services.AddTransient<LoginViewModel>();
        services.AddTransient<EstoqueViewModel>();
        services.AddTransient<GestaoFilaViewModel>();
        services.AddTransient<HomeColaboradorViewModel>();
        services.AddTransient<PerfilViewModel>();

        // ViewModel da sua colega
        services.AddTransient<CadastroViewModel>();
    }

    private static void RegistrarPaginas(IServiceCollection services)
    {
        services.AddTransient<LoginPage>();
        services.AddTransient<GestaoFilaPage>();
        services.AddTransient<EstoquePage>();
        services.AddTransient<MainPage>();
        services.AddTransient<HomeColaboradorPage>();
        services.AddTransient<PerfilPage>();

        // Página da sua colega
        services.AddTransient<CadastroPage>();
    }
}