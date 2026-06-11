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
        var apiBaseUrl = "http://187.90.210.145:5000/";

        services.AddTransient<AuthenticatedHttpHandler>();

        services.AddHttpClient<IAuthService, AuthService>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        });

        services.AddHttpClient<IFirebaseService, HttpFirebaseService>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
        })
        .AddHttpMessageHandler<AuthenticatedHttpHandler>()
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        });
    }

    private static void RegistrarServicos(IServiceCollection services)
    {
        services.AddScoped<INavigationService, NavigationService>();
        services.AddSingleton<ICartService, CartService>();
        services.AddSingleton<IDialogService, DialogService>();
    }

    private static void RegistrarViewModels(IServiceCollection services)
    {
        services.AddTransient<LoginViewModel>();
        services.AddTransient<EstoqueViewModel>();
        services.AddTransient<GestaoFilaViewModel>();
        services.AddTransient<HomeColaboradorViewModel>();
        services.AddTransient<PerfilViewModel>();
        services.AddTransient<CheckoutViewModel>();
        services.AddTransient<CadastroViewModel>();
        services.AddTransient<DetalheSolicitacaoViewModel>();
        services.AddTransient<AnaliseSolicitacaoViewModel>();
    }

    private static void RegistrarPaginas(IServiceCollection services)
    {
        services.AddTransient<LoginPage>();
        services.AddTransient<GestaoFilaPage>();
        services.AddTransient<EstoquePage>();
        services.AddTransient<MainPage>();
        services.AddTransient<HomeColaboradorPage>();
        services.AddTransient<PerfilPage>();
        services.AddTransient<CheckoutPage>();
        services.AddTransient<CadastroPage>();
        services.AddTransient<DetalheSolicitacaoPage>();
        services.AddTransient<AnaliseSolicitacaoPage>();
        services.AddSingleton<AppShell>();
    }
}