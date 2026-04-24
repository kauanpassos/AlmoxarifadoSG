using Microsoft.Extensions.Logging;
using Supabase;
using Almoxarifado.App.Views;

namespace Almoxarifado.App;


//Alterar arquivo pro Firebase
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var url = "URL BANCO DE DADOS";
        var key = "CHAVE_ANON_AQUI";

        var options = new SupabaseOptions
        {
            AutoConnectRealtime = true
        };

        builder.Services.AddSingleton(provider => new Supabase.Client(url, key, options));

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<EstoquePage>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<Almoxarifado.App.Views.GestaoFilaPage>();
        builder.Services.AddTransient<Almoxarifado.App.ViewModels.GestaoFilaViewModel>();

        return builder.Build();
    }
}