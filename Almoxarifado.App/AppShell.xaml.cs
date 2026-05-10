using Almoxarifado.App.Views;
using Microsoft.Maui.Controls;

namespace Almoxarifado.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(NovaSolicitacaoPage), typeof(NovaSolicitacaoPage));
        Routing.RegisterRoute(nameof(EstoquePage), typeof(EstoquePage));
        Routing.RegisterRoute(nameof(HomeColaboradorPage), typeof(HomeColaboradorPage));
        Routing.RegisterRoute(nameof(GestaoFilaPage), typeof(GestaoFilaPage));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
    }
}