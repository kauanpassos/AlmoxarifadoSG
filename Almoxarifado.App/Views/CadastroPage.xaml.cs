using Almoxarifado.App.ViewModels;

namespace Almoxarifado.App.Views;

public partial class CadastroPage : ContentPage
{
    public CadastroPage(CadastroViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void ToggleSenha_Clicked(object sender, EventArgs e)
    {
        EntrySenha.IsPassword = !EntrySenha.IsPassword;
        BtnToggleSenha.Source = EntrySenha.IsPassword ? "eye_closed.png" : "eye_open.png";
    }

    private void ToggleConfirmarSenha_Clicked(object sender, EventArgs e)
    {
        EntryConfirmarSenha.IsPassword = !EntryConfirmarSenha.IsPassword;
        BtnToggleConfirmarSenha.Source = EntryConfirmarSenha.IsPassword ? "eye_closed.png" : "eye_open.png";
    }
}
