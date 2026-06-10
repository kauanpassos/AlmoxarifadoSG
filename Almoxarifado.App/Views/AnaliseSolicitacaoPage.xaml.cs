using Almoxarifado.App.ViewModels;
using Microsoft.Maui.Controls;

namespace Almoxarifado.App.Views;

public partial class AnaliseSolicitacaoPage : ContentPage
{
    public AnaliseSolicitacaoPage(AnaliseSolicitacaoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}