using Almoxarifado.App.ViewModels;
using Microsoft.Maui.Controls;

namespace Almoxarifado.App.Views;

public partial class GestaoFilaPage : ContentPage
{
    private readonly GestaoFilaViewModel _viewModel;

    public GestaoFilaPage(GestaoFilaViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync ();
    }
}