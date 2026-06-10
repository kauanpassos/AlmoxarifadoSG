using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.App.Validations;
using Almoxarifado.Domain.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace Almoxarifado.App.ViewModels;

public partial class CadastroViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;

    public CadastroViewModel(IAuthService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
    }

    [ObservableProperty]
    private string _nome = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _setor = string.Empty;

    [ObservableProperty]
    private string _senha = string.Empty;

    [ObservableProperty]
    private string _confirmarSenha = string.Empty;

    [ObservableProperty]
    private TipoUsuario _tipo = TipoUsuario.Colaborador;

    public IList<TipoUsuario> TiposUsuarios { get; } = Enum.GetValues(typeof(TipoUsuario)).Cast<TipoUsuario>().ToList();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TemErro))]
    private string _mensagemErro = string.Empty;

    public bool TemErro => !string.IsNullOrWhiteSpace(MensagemErro);

    [ObservableProperty]
    private bool _isBusy;

    [RelayCommand]
    private async Task RegistrarAsync()
    {
        if (IsBusy)
            return;

        MensagemErro = string.Empty;

        var validator = new CadastroValidator();
        var validationResult = validator.Validate(this);

        if (!validationResult.IsValid)
        {
            MensagemErro = string.Join(Environment.NewLine, validationResult.Errors.Select(e => e.ErrorMessage));
            return;
        }

        try
        {
            IsBusy = true;

            var request = new Models.RegistrarUsuarioRequest(Nome, Email, Senha, Setor, Tipo);
            await _authService.RegistrarAsync(request);

            if (Shell.Current is not null)
            {
                await Shell.Current.DisplayAlert("Sucesso", "Usuário cadastrado com sucesso!", "OK");
            }

            await VoltarParaLoginAsync();
        }
        catch (Exception ex)
        {
            MensagemErro = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task VoltarParaLoginAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}