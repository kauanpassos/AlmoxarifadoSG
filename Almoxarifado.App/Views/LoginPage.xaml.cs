using Almoxarifado.App.ViewModels;

namespace Almoxarifado.App.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        BindingContext = viewModel;
    }
}