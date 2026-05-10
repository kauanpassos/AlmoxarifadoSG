using Almoxarifado.App.ViewModels;
using Microsoft.Maui.Controls;

namespace Almoxarifado.App.Views;

public partial class NovaSolicitacaoPage : ContentPage
{
    public NovaSolicitacaoPage(NovaSolicitacaoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}