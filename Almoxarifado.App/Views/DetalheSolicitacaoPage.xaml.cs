namespace Almoxarifado.App.Views;

public partial class DetalheSolicitacaoPage : ContentPage
{
    public DetalheSolicitacaoPage(ViewModels.DetalheSolicitacaoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}