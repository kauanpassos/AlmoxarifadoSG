using Almoxarifado.App.Services;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.App.Views;
using Almoxarifado.Domain.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Almoxarifado.App.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string _email = string.Empty;

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

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Aviso", "Preencha o e-mail e a senha.", "OK");
            return;
        }

        try
        {
            IsBusy = true;
            var user = await _authService.LoginAsync(Email, Password);

            if (user != null)
            {
                UsuarioSessao.UsuarioLogado = user;

                if (user.Tipo == TipoUsuario.Almoxarife)
                {
                    await Shell.Current.GoToAsync($"//{nameof(GestaoFilaPage)}");
                }
                else if (user.Tipo == TipoUsuario.Colaborador)
                {
                    await Shell.Current.GoToAsync($"//{nameof(HomeColaboradorPage)}");
                }
                else
                {
                    await Shell.Current.GoToAsync($"//{nameof(EstoquePage)}");
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Erro de Login", "Credenciais inválidas. Verifique seu e-mail e senha.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erro", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}