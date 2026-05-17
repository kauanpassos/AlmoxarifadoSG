using Almoxarifado.App.Services;
using Almoxarifado.App.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Almoxarifado.App.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    public bool IsNotBusy => !IsBusy;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var user = await _authService.LoginAsync(Username, Password);

            if (user != null)
            {
                if (user.Tipo == "Almoxarife")
                    await Shell.Current.GoToAsync("//GestaoFilaPage");
                else if (user.Tipo == "Colaborador")
                    await Shell.Current.GoToAsync("//HomeColaboradorPage");
                else
                    await Shell.Current.GoToAsync("//EstoquePage");
            }
            else
            {
                await Shell.Current.DisplayAlert("Erro de Login", "Credenciais inválidas. Verifique seu usuário e senha.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Aviso", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}