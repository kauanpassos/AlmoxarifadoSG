using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.App.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Almoxarifado.App.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string senha;

    [ObservableProperty]
    private bool isBusy;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    public async Task FazerLoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Senha))
        {
            await Application.Current.MainPage.DisplayAlert("Aviso", "Preencha e-mail e senha.", "OK");
            return;
        }

        IsBusy = true;

        var usuarioLogado = await _authService.LoginAsync(Email, Senha);

        IsBusy = false;

        if (usuarioLogado != null)
        {
            if (usuarioLogado.Tipo == "Almoxarife")
            {
                Application.Current.MainPage = IPlatformApplication.Current.Services.GetService<GestaoFilaPage>();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Sucesso", "Bem vindo Funcionario do Galpão!", "OK");
            }
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Erro", "E-mail ou senha incorretos.", "OK");
        }
    }
}