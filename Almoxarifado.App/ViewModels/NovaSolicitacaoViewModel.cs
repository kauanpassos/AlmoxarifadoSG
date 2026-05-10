using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Almoxarifado.App.ViewModels;

public partial class NovaSolicitacaoViewModel : ObservableObject
{
    [ObservableProperty]
    private string _iniciaisUsuario = string.Empty;

    [ObservableProperty]
    private int _quantidadeSolicitada = 1;

    [ObservableProperty]
    private int _estoqueDisponivel;

    [ObservableProperty]
    private ObservableCollection<string> _sugestoesRapidas = new();

    [RelayCommand]
    private void AumentarQuantidade()
    {
        QuantidadeSolicitada++;
    }

    [RelayCommand]
    private void DiminuirQuantidade()
    {
        if (QuantidadeSolicitada > 1)
        {
            QuantidadeSolicitada--;
        }
    }

    [RelayCommand]
    private async Task VoltarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task ConfirmarPedidoAsync()
    {
        await Shell.Current.DisplayAlert("Sucesso", $"Solicitação de {QuantidadeSolicitada} unidade(s) confirmada!", "OK");
        await Shell.Current.GoToAsync("..");
    }
}