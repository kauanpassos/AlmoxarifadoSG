using Microsoft.Maui.Controls;
using Almoxarifado.App.ViewModels;

namespace Almoxarifado.App.Views;

public partial class GestaoFilaPage : ContentPage
{
    public GestaoFilaPage(GestaoFilaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}