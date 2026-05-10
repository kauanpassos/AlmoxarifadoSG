using Almoxarifado.App.ViewModels;
using Microsoft.Maui.Controls;

namespace Almoxarifado.App.Views;

public partial class HomeColaboradorPage : ContentPage
{
    private readonly HomeColaboradorViewModel _viewModel;

    public HomeColaboradorPage(HomeColaboradorViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.CarregarDashboardCommand.CanExecute(null))
        {
            _viewModel.CarregarDashboardCommand.Execute(null);
        }
    }
}