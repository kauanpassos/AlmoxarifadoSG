using Microsoft.Maui.Controls;
using Almoxarifado.App.ViewModels;

namespace Almoxarifado.App.Views;

public partial class CheckoutPage : ContentPage
{
    public CheckoutPage(CheckoutViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
