using Almoxarifado.App.Models;
using Almoxarifado.App.Services;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.App.Views;
using Almoxarifado.Application.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Almoxarifado.App.ViewModels;

public sealed partial class GestaoFilaViewModel : ObservableObject
{
    private const string FiltroTodos = "Todos";
    private const string StatusProdutoSemNome = "Produto Sem Nome";

    private readonly IFirebaseService _firebaseService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;

    private List<FilaItemModel> _todosItensAchatados = new();

    public ObservableCollection<FilaItemModel> FilaAchatada { get; } = new();

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _textoPesquisa = string.Empty;

    [ObservableProperty]
    private string _filtroAtual = FiltroTodos;

    public bool IsEmpty => FilaAchatada.Count is 0 && IsBusy is false;

    public string IniciaisUsuario
    {
        get
        {
            var nome = UsuarioSessao.UsuarioLogado?.Nome;
            if (string.IsNullOrWhiteSpace(nome)) return "US";

            var partes = nome.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length is 1) return partes[0].Substring(0, Math.Min(2, partes[0].Length)).ToUpper();

            return $"{partes[0][0]}{partes[^1][0]}".ToUpper();
        }
    }

    public GestaoFilaViewModel(
        IFirebaseService firebaseService,
        INavigationService navigationService,
        IDialogService dialogService)
    {
        _firebaseService = firebaseService ?? throw new ArgumentNullException(nameof(firebaseService));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
    }

    partial void OnTextoPesquisaChanged(string value) => AplicarFiltros();
    partial void OnIsBusyChanged(bool value) => OnPropertyChanged(nameof(IsEmpty));

    [RelayCommand]
    private void MudarFiltro(string novoFiltro)
    {
        FiltroAtual = string.IsNullOrWhiteSpace(novoFiltro) ? FiltroTodos : novoFiltro;
        AplicarFiltros();
    }

    [RelayCommand]
    public async Task IrParaPerfilAsync()
    {
        await _navigationService.NavigateToAsync(nameof(PerfilPage));
    }

    [RelayCommand]
    private async Task IrParaEstoqueAsync()
    {
        await Shell.Current.GoToAsync($"//{nameof(EstoquePage)}");
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            _todosItensAchatados.Clear();

            var dados = await _firebaseService.GetSolicitacoesPendentesAsync();

            if (dados != null)
            {
                _todosItensAchatados = dados.SelectMany(solicitacao => solicitacao.Itens.Select(item => new FilaItemModel
                {
                    SolicitacaoId = solicitacao.Id,
                    UsuarioSolicitante = solicitacao.UsuarioId,
                    NomeProduto = string.IsNullOrWhiteSpace(item.NomeProduto) ? StatusProdutoSemNome : item.NomeProduto,
                    Sku = item.Sku,
                    Quantidade = item.Quantidade,
                    Status = solicitacao.Status,
                    SolicitacaoOriginal = solicitacao
                })).ToList();

                AplicarFiltros();
            }
        }
        catch (Exception)
        {
            await _dialogService.ShowAlertAsync("Erro de Comunicação", "Não foi possível carregar a fila de solicitações no momento.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void AplicarFiltros()
    {
        FilaAchatada.Clear();
        var query = _todosItensAchatados.AsEnumerable();

        if (FiltroAtual != FiltroTodos)
        {
            query = query.Where(x => x.Status.Equals(FiltroAtual, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(TextoPesquisa))
        {
            var termo = TextoPesquisa.ToLowerInvariant();
            query = query.Where(x =>
                x.NomeProduto.ToLowerInvariant().Contains(termo) ||
                x.Sku.ToLowerInvariant().Contains(termo) ||
                x.Status.ToLowerInvariant().Contains(termo));
        }

        foreach (var item in query)
        {
            FilaAchatada.Add(item);
        }

        OnPropertyChanged(nameof(IsEmpty));
    }

    [RelayCommand]
    private async Task AnalisarSolicitacaoAsync(FilaItemModel itemSelecionado)
    {
        if (itemSelecionado?.SolicitacaoOriginal == null) return;

        var parametros = new Dictionary<string, object>
        {
            { "SolicitacaoSelecionada", itemSelecionado.SolicitacaoOriginal }
        };

        await Shell.Current.GoToAsync(nameof(AnaliseSolicitacaoPage), parametros);
    }
}