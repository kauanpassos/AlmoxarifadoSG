using Almoxarifado.App.ViewModels;
using Microsoft.Maui.Controls;

namespace Almoxarifado.App.Views;

public partial class EstoquePage : ContentPage
{
    public EstoquePage(EstoqueViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}