using Microsoft.Extensions.Logging;
using Supabase;
using Almoxarifado.App.Views;

namespace Almoxarifado.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(static fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Supabase
            var url = "https://smbzclkloyaonqndioao.supabase.co";
            var chave = "sb_publishable_KQu31DyUG36voZwn8VOgIQ_p6Cp5BuJ";

            // tá criando conexão
            var options = new SupabaseOptions
            {
                AutoConnectRealtime = true
            };
            var supabaseClient = new Supabase.Client(url, chave, options);

            // guardando essa conexão para o app inteiro poder usar
            builder.Services.AddSingleton(supabaseClient);

            // Eu ensino o aplicativo a registrar a LoginPage para que ele possa abri-la e entregar o Supabase pra ela automaticamente.
            builder.Services.AddTransient<LoginPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}