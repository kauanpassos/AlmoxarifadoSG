using Almoxarifado.Domain;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Almoxarifado.App.ViewModels;

public partial class GestaoFilaViewModel : ObservableObject
{
    public ObservableCollection<Solicitacao> Pedidos { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    private bool isBusy;

    public bool IsEmpty => Pedidos.Count == 0 && !IsBusy;

    public async Task CarregarPedidosAsync()
    {
        IsBusy = true;
        Pedidos.Clear();
        await Task.Delay(1500);

        IsBusy = false;
    }
}