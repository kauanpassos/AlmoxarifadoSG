using Almoxarifado.App.Services;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.Domain;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Almoxarifado.App.ViewModels;
public partial class PerfilViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Iniciais))]
    [NotifyPropertyChangedFor(nameof(IsAlmoxarife))]
    private Usuario? _usuarioAtual;

    public string Iniciais => ObterIniciais(UsuarioAtual?.Nome);
    public bool IsAlmoxarife => UsuarioAtual?.Tipo == "Almoxarife";
    public bool IsColaborador => UsuarioAtual?.Tipo == "Colaborador";

    [RelayCommand]
    private async Task VoltarParaHomeAsync()
    {
        await _navigationService.NavigateToAsync("//HomeColaboradorPage");
    }

    public PerfilViewModel(IAuthService authService, INavigationService navigationService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

        UsuarioAtual = UsuarioSessao.UsuarioLogado;
    }

    [RelayCommand]
    private async Task SairAsync()
    {
        try
        {
            bool confirmar = await Shell.Current.DisplayAlert(
                "Sair",
                "Deseja realmente encerrar sua sessão?",
                "Sair",
                "Cancelar");

            if (confirmar)
            {
                await _navigationService.NavigateToLoginAsync();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao sair: {ex.Message}\n{ex.StackTrace}");

            await Shell.Current.DisplayAlert(
                "Erro",
                "Ocorreu um erro ao sair. Tente novamente.",
                "OK");
        }
    }

    [RelayCommand]
    private async Task VoltarParaFilaAsync()
    {
        await _navigationService.NavigateToAsync("//GestaoFilaPage");
    }

    private string ObterIniciais(string? nome)
    {
        if (string.IsNullOrWhiteSpace(nome)) return "US";
        var partes = nome.Trim().Split(' ');
        if (partes.Length == 1) return partes[0].Substring(0, Math.Min(2, partes[0].Length)).ToUpper();
        return $"{partes[0][0]}{partes[^1][0]}".ToUpper();
    }
}