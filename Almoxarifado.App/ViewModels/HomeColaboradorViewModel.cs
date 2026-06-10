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

public partial class HomeColaboradorViewModel : ObservableObject
{
    private const string StatusFiltroTodos = "Todas";
    private const string StatusCancelada = "Cancelada";

    private readonly INavigationService _navigationService;
    private readonly IFirebaseService _firebaseService;

    private List<SolicitacaoDto> _solicitacoesOriginais = new();
    private List<SolicitacaoModel> _modelosEmMemoria = new();

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _iniciaisUsuario = string.Empty;

    [ObservableProperty]
    private string _termoPesquisa = string.Empty;

    [ObservableProperty]
    private string _filtroStatusAtual = StatusFiltroTodos;

    public ObservableCollection<SolicitacaoModel> ListaSolicitacoes { get; } = new();

    public HomeColaboradorViewModel(INavigationService navigationService, IFirebaseService firebaseService)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _firebaseService = firebaseService ?? throw new ArgumentNullException(nameof(firebaseService));

        var usuarioLogado = UsuarioSessao.UsuarioLogado;
        if (usuarioLogado is not null)
        {
            IniciaisUsuario = ObterIniciais(usuarioLogado.Nome);
        }
    }

    partial void OnTermoPesquisaChanged(string value)
    {
        AplicarFiltros();
    }

    [RelayCommand]
    private void AlterarFiltro(string status)
    {
        FiltroStatusAtual = string.IsNullOrWhiteSpace(status) ? StatusFiltroTodos : status;
        AplicarFiltros();
    }

    [RelayCommand]
    private async Task IrParaPerfilAsync() => await _navigationService.NavigateToAsync(nameof(PerfilPage));

    [RelayCommand]
    private async Task VerEstoqueAsync() => await Shell.Current.GoToAsync($"//{nameof(EstoquePage)}");

    [RelayCommand]
    public async Task CarregarDashboardAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            _solicitacoesOriginais.Clear();
            _modelosEmMemoria.Clear();

            var usuario = UsuarioSessao.UsuarioLogado;
            if (usuario is null) return;

            var solicitacoes = await _firebaseService.GetSolicitacoesUsuarioAsync(usuario.Id);

            if (solicitacoes is not null)
            {
                _solicitacoesOriginais = solicitacoes.Where(s => s.Status != StatusCancelada).ToList();

                _modelosEmMemoria = _solicitacoesOriginais.SelectMany(s => s.Itens.Select(i => new SolicitacaoModel
                {
                    NomeProduto = string.IsNullOrWhiteSpace(i.NomeProduto) ? i.Sku : i.NomeProduto,
                    Sku = i.Sku,
                    Quantidade = i.Quantidade,
                    NomeStatus = s.Status
                })).ToList();

                AplicarFiltros();
            }
        }
        catch (Exception)
        {
            var toast = CommunityToolkit.Maui.Alerts.Toast.Make("Falha na rede. Não foi possível carregar o histórico.", CommunityToolkit.Maui.Core.ToastDuration.Long);
            await toast.Show();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void AplicarFiltros()
    {
        ListaSolicitacoes.Clear();
        var query = _modelosEmMemoria.AsEnumerable();

        if (FiltroStatusAtual != StatusFiltroTodos)
        {
            query = query.Where(q => q.NomeStatus.Equals(FiltroStatusAtual, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(TermoPesquisa))
        {
            var termo = TermoPesquisa.ToLowerInvariant();
            query = query.Where(q => q.NomeProduto.ToLowerInvariant().Contains(termo) ||
                                     q.Sku.ToLowerInvariant().Contains(termo));
        }

        foreach (var item in query)
        {
            ListaSolicitacoes.Add(item);
        }
    }

    [RelayCommand]
    private async Task AbrirDetalhesSolicitacaoAsync(SolicitacaoModel itemSelecionado)
    {
        if (itemSelecionado is null) return;

        var solicitacao = _solicitacoesOriginais.FirstOrDefault(s =>
            s.Status == itemSelecionado.NomeStatus &&
            s.Itens.FirstOrDefault(i => i.Sku == itemSelecionado.Sku) is not null);

        if (solicitacao is not null)
        {
            var parametros = new Dictionary<string, object>
            {
                { "SolicitacaoSelecionada", solicitacao }
            };

            await Shell.Current.GoToAsync(nameof(DetalheSolicitacaoPage), parametros);
        }
    }

    private static string ObterIniciais(string? nome)
    {
        if (string.IsNullOrWhiteSpace(nome)) return "CO";

        var partes = nome.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        return partes.Length is 1
            ? partes[0][..Math.Min(2, partes[0].Length)].ToUpper()
            : $"{partes[0][0]}{partes[^1][0]}".ToUpper();
    }
}