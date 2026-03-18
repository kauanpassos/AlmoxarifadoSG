using Microsoft.Extensions.Logging;
using Supabase;
using Almoxarifado.App.Views;

namespace Almoxarifado.App;

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

        // --- CONFIGURAÇÃO DO SUPABASE ---
        // Substitua pelas suas credenciais reais do painel do Supabase
        var url = "https://SUA_URL_AQUI.supabase.co";
        var key = "SUA_CHAVE_ANON_AQUI";

        var options = new SupabaseOptions
        {
            AutoConnectRealtime = true
        };

        // Registra o cliente do Supabase como Singleton (uma única instância para o app todo)
        builder.Services.AddSingleton(provider => new Supabase.Client(url, key, options));

        // --- REGISTRO DAS PÁGINAS (Essencial para o login funcionar!) ---
        // Isso permite que a LoginPage receba o 'Supabase.Client' no construtor
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<EstoquePage>();
        builder.Services.AddTransient<MainPage>();

        return builder.Build();
    }
}