using Almoxarifado.App.ViewModels;
using Microsoft.Maui.Controls;

namespace Almoxarifado.App.Views;

public partial class PerfilPage : ContentPage
{
    public PerfilPage(PerfilViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}