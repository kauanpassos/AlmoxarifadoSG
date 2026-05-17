using Almoxarifado.App.Services;
using Almoxarifado.Domain;
using Almoxarifado.Domain.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Almoxarifado.App.ViewModels;

public partial class GestaoFilaViewModel : ObservableObject
{
    private readonly IReadOnlyRepository<Solicitacao> _repository;

    private readonly List<Solicitacao> _filaCompleta = new();

    public ObservableCollection<Solicitacao> Solicitacoes { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    private bool isBusy;

    [ObservableProperty]
    private string textoPesquisa = string.Empty;

    [ObservableProperty]
    private string filtroStatus = "Todos";

    public bool IsEmpty => Solicitacoes.Count == 0 && !IsBusy;

    public string IniciaisUsuario
    {
        get
        {
            var nome = UsuarioSessao.UsuarioLogado?.Nome;
            if (string.IsNullOrWhiteSpace(nome))
                return "US";

            var partes = nome.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (partes.Length == 1)
                return partes[0].Substring(0, Math.Min(2, partes[0].Length)).ToUpper();

            return $"{partes[0][0]}{partes[^1][0]}".ToUpper();
        }
    }

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

    [RelayCommand]
    private async Task IrParaPerfilAsync()
    {
        await Shell.Current.GoToAsync(nameof(Views.PerfilPage));
    }

    partial void OnTextoPesquisaChanged(string value) => AplicarFiltros();

    public async Task CarregarSolicitacoesAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            Solicitacoes.Clear();

            var usuario = UsuarioSessao.UsuarioLogado;

            IEnumerable<Solicitacao> dados;

            if (usuario == null) return;

            if (usuario.Tipo == "Almoxarife")
            {
                dados = await _repository.GetAllAsync();
            }
            else
            {
                dados = await _repository.GetByUserIdAsync(usuario.Id);
            }

            _filaCompleta.Clear();
            _filaCompleta.AddRange(dados);

            AplicarFiltros();
        }
        catch (Exception ex)
        {
            await Microsoft.Maui.Controls.Application.Current!.MainPage!.DisplayAlert("Erro ao carregar", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void AplicarFiltros()
    {
        var temp = _filaCompleta.AsEnumerable();

        if (FiltroStatus != "Todos")
        {
            temp = temp.Where(s => s.Status.Equals(FiltroStatus, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(TextoPesquisa))
        {
            temp = temp.Where(s =>
                s.Id.Contains(TextoPesquisa, StringComparison.OrdinalIgnoreCase) ||
                (s.Observacao != null && s.Observacao.Contains(TextoPesquisa, StringComparison.OrdinalIgnoreCase)) ||
                (s.Sku != null && s.Sku.Contains(TextoPesquisa, StringComparison.OrdinalIgnoreCase)));
        }

        Solicitacoes.Clear();
        foreach (var item in temp)
        {
            Solicitacoes.Add(item);
        }

        OnPropertyChanged(nameof(IsEmpty));
    }
}