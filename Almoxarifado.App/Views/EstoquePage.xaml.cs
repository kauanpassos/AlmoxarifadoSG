using Almoxarifado.App.ViewModels;

namespace Almoxarifado.App.Views;

public partial class EstoquePage : ContentPage
{
    private readonly EstoqueViewModel _viewModel;

    public EstoquePage(EstoqueViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadItemsCommand.Execute(null);
    }
}