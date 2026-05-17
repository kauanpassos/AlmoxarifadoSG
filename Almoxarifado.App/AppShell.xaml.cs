using Almoxarifado.App.Views;
using Almoxarifado.App.Services.Interfaces;
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
                if (user.Tipo == "Almoxarife")
                    await Current.GoToAsync($"//{nameof(GestaoFilaPage)}");
                else if (user.Tipo == "Colaborador")
                    await Current.GoToAsync($"//{nameof(HomeColaboradorPage)}");
                else
                    await Current.GoToAsync($"//{nameof(EstoquePage)}");
            }
        }
    }
}