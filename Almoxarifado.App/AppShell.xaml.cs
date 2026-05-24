using Almoxarifado.App.Views;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.Domain.Enums;
using Microsoft.Maui.Controls;

namespace Almoxarifado.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(PerfilPage), typeof(PerfilPage));

        Dispatcher.DispatchAsync(ChecarSessaoE_RoteamentoAsync);
    }

    private async Task ChecarSessaoE_RoteamentoAsync()
    {
        var authService = IPlatformApplication.Current?.Services.GetService<IAuthService>();

        if (authService != null)
        {
            var user = await authService.VerificarSessaoAtivaAsync();

            if (user != null)
            {
                if (user.Tipo == TipoUsuario.Almoxarife)
                    await Current.GoToAsync($"//{nameof(GestaoFilaPage)}");
                else if (user.Tipo == TipoUsuario.Colaborador)
                    await Current.GoToAsync($"//{nameof(HomeColaboradorPage)}");
                else
                    await Current.GoToAsync($"//{nameof(EstoquePage)}");
            }
        }
    }
}