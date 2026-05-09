using Microsoft.Extensions.Logging;
using Firebase.Database;
using Almoxarifado.App.Views;
using Almoxarifado.App.ViewModels;
using Almoxarifado.App.Services;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.Domain;
using Almoxarifado.Domain.Interfaces;

namespace Almoxarifado.App;

// Coração da configuração do nosso App Mobile. 
// Aqui é onde registramos TUDO que o aplicativo precisa para rodar (Fontes, Banco, Telas).
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                // Registramos as fontes customizadas para que fiquem disponíveis globalmente no XAML.
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Habilita logs de debug apenas quando estamos desenvolvendo.
#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Configuração do Cliente Firebase. 
        // OBS: A URL deve vir de um arquivo de configuração seguro em um cenário real.
        var firebaseUrl = "URL_DO_FIREBASE_AQUI";
        var firebaseClient = new FirebaseClient(firebaseUrl);
        builder.Services.AddSingleton(firebaseClient);

        // CONFIGURAÇÃO DE INFRA: Registramos os motores de acesso a dados.
        // Usamos Scoped para garantir um ciclo de vida controlado durante a navegação.
        builder.Services.AddScoped<IEngine<Estoque>>(sp => new FirebaseEngine<Estoque>(firebaseClient, "estoque"));
        builder.Services.AddScoped<IReadOnlyRepository<Estoque>>(sp => sp.GetRequiredService<IEngine<Estoque>>());
        builder.Services.AddScoped<IWriteOnlyRepository<Estoque>>(sp => sp.GetRequiredService<IEngine<Estoque>>());

        // Por enquanto usando um Mock para agilizar o desenvolvimento da UI sem depender de API.
        builder.Services.AddScoped<IAuthService, MockAuthService>();

        // REGISTRO DE VIEWMODELS: Usamos Transient para que o estado da ViewModel 
        // seja resetado toda vez que o usuário navegar para a tela.
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<EstoqueViewModel>();
        builder.Services.AddTransient<GestaoFilaViewModel>();

        // REGISTRO DE VIEWS: Também Transient, acompanhando suas respectivas ViewModels.
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<EstoquePage>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<GestaoFilaPage>();

        return builder.Build();
    }
}

// Implementação fake para não travar o desenvolvimento das telas de Login.
public class MockAuthService : IAuthService
{
    public Task<Almoxarifado.Domain.Usuario?> LoginAsync(string u, string p) 
        => Task.FromResult<Almoxarifado.Domain.Usuario?>(new Almoxarifado.Domain.Usuario { Nome = "Usuário Teste" });
        
    public Task LogoutAsync() => Task.CompletedTask;
}