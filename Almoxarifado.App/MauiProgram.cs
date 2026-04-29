using Almoxarifado.App.Services;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.App.ViewModels;
using Almoxarifado.App.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace Almoxarifado.App;


//Alterar arquivo pro Firebase
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
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IFirebaseService, FirebaseService>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<GestaoFilaViewModel>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<GestaoFilaPage>();
        builder.Services.AddTransient<EstoquePage>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<Almoxarifado.App.Views.GestaoFilaPage>();
        builder.Services.AddTransient<Almoxarifado.App.ViewModels.GestaoFilaViewModel>();

        return builder.Build();
    }
}