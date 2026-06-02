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
        var apiBaseUrl = DeviceInfo.Platform == DevicePlatform.Android
            ? "https://10.0.2.2:7185/"
            : "https://localhost:7185/";

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

        // ADICIONADO: Seus serviços vitais de Estoque
        services.AddScoped<IFirebaseService, HttpFirebaseService>();
        services.AddSingleton<ICartService, CartService>();
    }

    private static void RegistrarViewModels(IServiceCollection services)
    {
        services.AddTransient<LoginViewModel>();
        services.AddTransient<EstoqueViewModel>();
        services.AddTransient<GestaoFilaViewModel>();
        services.AddTransient<HomeColaboradorViewModel>();
        services.AddTransient<NovaSolicitacaoViewModel>();
        services.AddTransient<PerfilViewModel>();

        // ADICIONADO: ViewModel da sua colega
        services.AddTransient<CadastroViewModel>();
    }

    private static void RegistrarPaginas(IServiceCollection services)
    {
        services.AddTransient<LoginPage>();
        services.AddTransient<GestaoFilaPage>();
        services.AddTransient<EstoquePage>();
        services.AddTransient<MainPage>();
        services.AddTransient<HomeColaboradorPage>();
        services.AddTransient<NovaSolicitacaoPage>();
        services.AddTransient<PerfilPage>();

        // ADICIONADO: Página da sua colega
        services.AddTransient<CadastroPage>();
    }
}