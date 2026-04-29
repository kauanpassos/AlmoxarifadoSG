using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Almoxarifado.Domain;

namespace Almoxarifado.App.Views
{
    public partial class EstoquePage : ContentPage
    {
        public ObservableCollection<Estoque> PecasNoEstoque { get; set; } = new ObservableCollection<Estoque>();
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Eu chamo a minha função de buscar os dados no banco assim que a tela abre.
            await CarregarEstoque();
        }

        private async Task CarregarEstoque()
        {
            try
            {
            
            }
            catch (System.Exception)
            {
                // Se der ruim (tipo sem internet), eu aviso o usuário.
                await DisplayAlert("Erro", "Não foi possível carregar o estoque. Verifique sua conexão.", "OK");
            }
        }
    }
}