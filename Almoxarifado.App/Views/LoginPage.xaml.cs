using Almoxarifado.App.ViewModels;
using Microsoft.Maui.Controls;

namespace Almoxarifado.App.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}