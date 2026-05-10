using Almoxarifado.Domain;
using Almoxarifado.Domain.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;

namespace Almoxarifado.App.ViewModels;

public partial class GestaoFilaViewModel : ObservableObject
{
    private readonly IReadOnlyRepository<Solicitacao> _repository;

    // Lista interna para cache dos dados vindos do banco
    private readonly List<Solicitacao> _filaCompleta = new();

    // Lista vinculada à UI para exibir as solicitações pendentes
    public ObservableCollection<Solicitacao> Solicitacoes { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    private bool isBusy;

    // Campos do Filtro
    [ObservableProperty]
    private string textoPesquisa = string.Empty;

    [ObservableProperty]
    private string filtroStatus = "Todos";

    public bool IsEmpty => Solicitacoes.Count == 0 && !IsBusy;

    // Comandos para a UI
    public ICommand LoadCommand { get; }
    public ICommand MudarFiltroCommand { get; }

    public GestaoFilaViewModel(IReadOnlyRepository<Solicitacao> repository)
    {
        _repository = repository;

        LoadCommand = new Command(async () => await CarregarSolicitacoesAsync());
        MudarFiltroCommand = new Command<string>(status =>
        {
            FiltroStatus = status;
            AplicarFiltros();
        });
    }
    partial void OnTextoPesquisaChanged(string value) => AplicarFiltros();

    public async Task CarregarSolicitacoesAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            Solicitacoes.Clear();

            // Agora falando com o Firebase via IReadOnlyRepository
            var dados = await _repository.GetAllAsync();

            _filaCompleta.Clear();
            _filaCompleta.AddRange(dados);

            AplicarFiltros();
        }
        catch (Exception ex)
        {
            await Microsoft.Maui.Controls.Application.Current!.MainPage!.DisplayAlert("Erro ao carregar solicitações", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void AplicarFiltros()
    {
        var temp = _filaCompleta.AsEnumerable();

        // Filtro por Status
        if (FiltroStatus != "Todos")
        {
            temp = temp.Where(s => s.Status.Equals(FiltroStatus, StringComparison.OrdinalIgnoreCase));
        }

        // Filtro por Texto
        if (!string.IsNullOrWhiteSpace(TextoPesquisa))
        {
            temp = temp.Where(s =>
                s.Id.Contains(TextoPesquisa, StringComparison.OrdinalIgnoreCase) ||
                (s.Observacao != null && s.Observacao.Contains(TextoPesquisa, StringComparison.OrdinalIgnoreCase)));
        }

        Solicitacoes.Clear();
        foreach (var item in temp)
        {
            Solicitacoes.Add(item);
        }

        OnPropertyChanged(nameof(IsEmpty));
    }
}