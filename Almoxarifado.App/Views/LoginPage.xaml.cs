using Almoxarifado.App.ViewModels;
using Microsoft.Maui.Controls;
using System;

namespace Almoxarifado.App.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void ToggleSenha_Clicked(object sender, EventArgs e)
    {
        if (EntrySenha != null && BtnToggleSenha != null)
        {
            EntrySenha.IsPassword = !EntrySenha.IsPassword;
            BtnToggleSenha.Source = EntrySenha.IsPassword ? "eye_closed.png" : "eye_open.png";
        }
    }
}