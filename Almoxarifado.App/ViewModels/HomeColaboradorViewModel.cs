using Almoxarifado.App.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Almoxarifado.App.ViewModels;

public partial class HomeColaboradorViewModel : ObservableObject
{
    [ObservableProperty]
    private string _iniciaisUsuario = string.Empty;

    [ObservableProperty]
    private string _termoPesquisa = string.Empty;

    public ObservableCollection<SolicitacaoModel> ListaSolicitacoes { get; } = new();

    public HomeColaboradorViewModel()
    {
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
}

public class SolicitacaoModel
{
    public string NomePeca { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public string NomeStatus { get; set; } = string.Empty;
    public string CorHexStatus { get; set; } = string.Empty;
}