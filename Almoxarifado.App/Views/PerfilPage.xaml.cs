using Almoxarifado.App.ViewModels;

namespace Almoxarifado.App.Views;

public partial class PerfilPage : ContentPage
{
    public PerfilPage(PerfilViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}