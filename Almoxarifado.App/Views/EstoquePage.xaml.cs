using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Almoxarifado.Domain;

namespace Almoxarifado.App.Views
{
    public partial class EstoquePage : ContentPage
    {
        // Eu crio essa variável pra guardar a conexão com o banco de dados.
        private readonly Supabase.Client _supabaseClient;

        // Eu crio essa lista especial do MAUI (ObservableCollection). Toda vez que eu colocar uma peça nova aqui, a tela atualiza sozinha!
        public ObservableCollection<Estoque> PecasNoEstoque { get; set; } = new ObservableCollection<Estoque>();

        // Eu recebo o Supabase no construtor igual fiz na tela de Login.
        public EstoquePage(Supabase.Client supabaseClient)
        {
            InitializeComponent();
            _supabaseClient = supabaseClient;

            // Eu aviso pra minha tela (XAML) que os dados que ela vai mostrar (Binding) vêm deste próprio arquivo C#.
            BindingContext = this;
        }

        // Eu uso esse método nativo do MAUI. Ele roda sozinho toda vez que a tela aparece pro usuário.
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
                // Eu vou lá no Supabase, na tabela de Estoque, e peço pra ele me trazer todas as linhas cadastradas.
                var resposta = await _supabaseClient.From<Estoque>().Get();

                // Eu limpo a lista antes de preencher pra não duplicar as coisas na tela caso o cara puxe pra atualizar.
                PecasNoEstoque.Clear();

                // Eu pego a lista que o Supabase me devolveu e jogo um por um dentro da minha lista que está ligada na tela.
                foreach (var peca in resposta.Models)
                {
                    PecasNoEstoque.Add(peca);
                }
            }
            catch (System.Exception)
            {
                // Se der ruim (tipo sem internet), eu aviso o usuário.
                await DisplayAlert("Erro", "Não foi possível carregar o estoque. Verifique sua conexão.", "OK");
            }
        }
    }
}