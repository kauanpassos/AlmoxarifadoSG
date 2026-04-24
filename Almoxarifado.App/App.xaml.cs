using Almoxarifado.App.Views;
using Microsoft.Maui.Controls;

namespace Almoxarifado.App
{
    public partial class App : Application
    {
        public App(LoginPage loginPage)
        {
            InitializeComponent();

            MainPage = loginPage;
        }
    }
}