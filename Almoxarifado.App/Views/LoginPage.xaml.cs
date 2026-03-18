using Almoxarifado.App.Services;

public partial class LoginPage : ContentPage
{
    private readonly IAuthService _authService; //usando o Auth

    public LoginPage(IAuthService authService)
    {
        _authService = authService;
        InitializeComponent();
    }

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        var usuario = await _authService.LoginAsync(EntryUsuario.Text, EntrySenha.Text);

        if (usuario != null)
        {
            await Shell.Current.GoToAsync("//EstoquePage");
        }
        else
        {
            await DisplayAlert("Erro", "Usuário ou senha inválidos", "OK");
        }
    }
}