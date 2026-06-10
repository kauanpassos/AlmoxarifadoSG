using Almoxarifado.App.Views;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.App.Services;
using Almoxarifado.Domain.Enums;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace Almoxarifado.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(PerfilPage), typeof(PerfilPage));
        Routing.RegisterRoute(nameof(CadastroPage), typeof(CadastroPage));
        Routing.RegisterRoute(nameof(EstoquePage), typeof(EstoquePage));
        Routing.RegisterRoute(nameof(CheckoutPage), typeof(CheckoutPage));
        Routing.RegisterRoute(nameof(DetalheSolicitacaoPage), typeof(DetalheSolicitacaoPage));
        Routing.RegisterRoute(nameof(AnaliseSolicitacaoPage), typeof(AnaliseSolicitacaoPage));
    }
}