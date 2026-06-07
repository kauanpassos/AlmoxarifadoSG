using Almoxarifado.App.ViewModels;
using Microsoft.Maui.Controls;

namespace Almoxarifado.App.Views;

public partial class CheckoutPage : ContentPage
{
    public CheckoutPage(CheckoutViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}