using Almoxarifado.App.Views; // Adicione isso lá em cima!
using Microsoft.Maui.Controls;

namespace Almoxarifado.App
{
    public partial class App : Application
    {
        // MUDE DE "public App()" PARA ISSO AQUI:
        public App(LoginPage loginPage)
        {
            InitializeComponent();

            // SUBSTITUA O QUE ESTIVER AQUI DENTRO POR ISSO:
            MainPage = loginPage;
        }
    }
}