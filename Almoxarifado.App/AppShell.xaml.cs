using Almoxarifado.App.Views;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.App.Services;
using Almoxarifado.Domain.Enums;
using Microsoft.Maui.Controls;

namespace Almoxarifado.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(PerfilPage), typeof(PerfilPage));
        Routing.RegisterRoute(nameof(CadastroPage), typeof(CadastroPage));

        Loaded += (s, e) => Dispatcher.DispatchAsync(ChecarSessaoERoteamentoAsync);
    }

    private async Task ChecarSessaoERoteamentoAsync()
    {
        var authService = Handler?.MauiContext?.Services.GetService<IAuthService>();

        if (authService is null)
            return;

        var user = await authService.VerificarSessaoAtivaAsync();

        if (user is null)
            return;

        UsuarioSessao.UsuarioLogado = user;

        var rota = user.Tipo switch
        {
            TipoUsuario.Almoxarife => $"//{nameof(GestaoFilaPage)}",
            TipoUsuario.Colaborador => $"//{nameof(HomeColaboradorPage)}",
            _ => $"//{nameof(EstoquePage)}"
        };

        await Current.GoToAsync(rota);
    }
} 