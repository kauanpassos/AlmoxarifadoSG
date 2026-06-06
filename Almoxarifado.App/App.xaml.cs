using Microsoft.Maui.Controls;

namespace Almoxarifado.App;
public partial class App : Microsoft.Maui.Controls.Application
{
    public App(AppShell shell)
    {
        InitializeComponent();
        MainPage = shell;
    }
}