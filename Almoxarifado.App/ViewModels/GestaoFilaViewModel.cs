using Almoxarifado.Domain;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Almoxarifado.App.ViewModels;

public partial class GestaoFilaViewModel : ObservableObject
{
    // Lista vinculada à UI para exibir as solicitações pendentes.
    public ObservableCollection<Solicitacao> Solicitacoes { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    private bool isBusy;

    public bool IsEmpty => Solicitacoes.Count == 0 && !IsBusy;

    public async Task CarregarSolicitacoesAsync()
    {
        IsBusy = true;
        Solicitacoes.Clear();
        
        // Simulação de carregamento. No futuro, falará com a API/Firebase via SolicitacaoController.
        await Task.Delay(1500);

        IsBusy = false;
    }
}