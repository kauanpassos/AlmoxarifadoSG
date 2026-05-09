using Almoxarifado.App.Services.Interfaces;
using System.Windows.Input;

namespace Almoxarifado.App.ViewModels;

public sealed class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private string _username = string.Empty;
    private string _password = string.Empty;

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public ICommand LoginCommand { get; }

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
        Title = "Login";
        LoginCommand = new Command(ExecuteLoginCommand);
    }

    private async void ExecuteLoginCommand()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var user = await _authService.LoginAsync(Username, Password);

            if (user != null)
            {
                await Shell.Current.GoToAsync("//EstoquePage");
            }
            else
            {
                await Application.Current!.MainPage!.DisplayAlert("Erro", "Usuário ou senha inválidos", "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert("Erro", $"Falha na autenticação: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
