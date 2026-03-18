using Almoxarifado.Domain;

namespace Almoxarifado.App.Views;

public partial class LoginPage : ContentPage
{
    private readonly Supabase.Client _supabase;

    public LoginPage(Supabase.Client supabase)
    {
        InitializeComponent();
        _supabase = supabase;
    }

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        // captura os dados usando os nomes x:Name que definiu
        string usuarioDigitado = EntryUsuario.Text;
        string senhaDigitada = EntrySenha.Text;

        if (string.IsNullOrWhiteSpace(usuarioDigitado) || string.IsNullOrWhiteSpace(senhaDigitada))
        {
            await DisplayAlert("Atenção", "Por favor, preencha o usuário e a senha.", "OK");
            return;
        }

        try
        {
            // tá buscando o usuário na tabela 'usuarios' do Supabase
            var response = await _supabase
                .From<Usuario>()
                .Where(x => x.Username == usuarioDigitado)
                .Where(x => x.Password == senhaDigitada)
                .Get();

            var usuario = response.Model;

            if (usuario != null)
            {
               
                await Shell.Current.GoToAsync("//EstoquePage");
            }
            else
            {
                await DisplayAlert("Erro", "Usuário ou senha incorretos.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro de Conexão", $"Não foi possível conectar ao banco: {ex.Message}", "OK");
        }
    }
}