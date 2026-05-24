using Almoxarifado.App.Services;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.App.Views;
using Almoxarifado.Domain;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Almoxarifado.App.ViewModels;

public partial class HomeColaboradorViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string _iniciaisUsuario = string.Empty;

    [ObservableProperty]
    private string _termoPesquisa = string.Empty;

    public ObservableCollection<SolicitacaoModel> ListaSolicitacoes { get; } = new();

    public HomeColaboradorViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

        var usuarioLogado = UsuarioSessao.UsuarioLogado;
        if (usuarioLogado != null)
        {
            IniciaisUsuario = ObterIniciais(usuarioLogado.Nome);
        }
    }

    [RelayCommand]
    private async Task IrParaPerfilAsync()
    {
        await _navigationService.NavigateToAsync(nameof(PerfilPage));
    }

    [RelayCommand]
    private async Task NovoPedidoAsync()
    {
        await Shell.Current.GoToAsync("///NovaSolicitacaoPage");
    }

    [RelayCommand]
    private async Task VerEstoqueAsync()
    {
        await Shell.Current.GoToAsync("///EstoquePage");
    }

    [RelayCommand]
    public async Task CarregarDashboardAsync()
    {
        ListaSolicitacoes.Clear();
    }

    private string ObterIniciais(string? nome)
    {
        if (string.IsNullOrWhiteSpace(nome)) return "CO";
        var partes = nome.Trim().Split(' ');
        if (partes.Length == 1) return partes[0].Substring(0, Math.Min(2, partes[0].Length)).ToUpper();
        return $"{partes[0][0]}{partes[^1][0]}".ToUpper();
    }
}

public class SolicitacaoModel
{
    public string NomePeca { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public string NomeStatus { get; set; } = string.Empty;
    public string CorHexStatus { get; set; } = string.Empty;
}