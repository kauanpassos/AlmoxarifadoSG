using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Almoxarifado.Domain;

namespace Almoxarifado.App.Views
{
    public partial class EstoquePage : ContentPage
    {
        private readonly Supabase.Client _supabaseClient;

        public ObservableCollection<Estoque> PecasNoEstoque { get; set; } = new ObservableCollection<Estoque>();

        public EstoquePage(Supabase.Client supabaseClient)
        {
            InitializeComponent();
            _supabaseClient = supabaseClient;
            BindingContext = this;
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CarregarEstoque();
        }

        private async Task CarregarEstoque()
        {
            try
            {

                var resposta = await _supabaseClient.From<Estoque>().Get();

                PecasNoEstoque.Clear();

                foreach (var peca in resposta.Models)
                {
                    PecasNoEstoque.Add(peca);
                }
            }
            catch (System.Exception)
            {
                await DisplayAlert("Erro", "Não foi possível carregar o estoque. Verifique sua conexão.", "OK");
            }
        }
    }
}