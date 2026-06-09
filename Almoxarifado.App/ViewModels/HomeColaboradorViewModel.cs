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
    private readonly INavigationService _navigationService;
    private readonly IFirebaseService _firebaseService;

    private List<SolicitacaoDto> _solicitacoesOriginais = new();

    [ObservableProperty]
    private string _iniciaisUsuario = string.Empty;

    [ObservableProperty]
    private string _termoPesquisa = string.Empty;

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

    [RelayCommand]
    private async Task IrParaPerfilAsync() => await _navigationService.NavigateToAsync(nameof(PerfilPage));

    [RelayCommand]
    private async Task VerEstoqueAsync() => await Shell.Current.GoToAsync($"//{nameof(EstoquePage)}");

    [RelayCommand]
    public async Task CarregarDashboardAsync()
    {
        try
        {
            ListaSolicitacoes.Clear();
            _solicitacoesOriginais.Clear();

            var usuario = UsuarioSessao.UsuarioLogado;
            if (usuario is null) return;

            var solicitacoes = await _firebaseService.GetSolicitacoesUsuarioAsync(usuario.Id);

            if (solicitacoes != null)
            {
                // 🔥 CORREÇÃO AQUI: Filtramos para ignorar as solicitações canceladas
                var solicitacoesAtivas = solicitacoes.Where(s => s.Status != "Cancelado").ToList();

                _solicitacoesOriginais = solicitacoesAtivas;

                // Construímos o modelo usando o NomeProduto quando disponível, caso contrário mostramos o Sku
                var modelos = solicitacoesAtivas.SelectMany(s => s.Itens.Select(i => new SolicitacaoModel
                {
                    NomeProduto = string.IsNullOrWhiteSpace(i.NomeProduto) ? i.Sku : i.NomeProduto,
                    Sku = i.Sku,
                    Quantidade = i.Quantidade,
                    NomeStatus = s.Status
                }));

                foreach (var model in modelos)
                {
                    ListaSolicitacoes.Add(model);
                }
            }
        }
        catch (Exception)
        {
            if (Microsoft.Maui.Controls.Application.Current?.MainPage is not null)
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Erro", "Não foi possível carregar o dashboard.", "OK");
        }
    }

    [RelayCommand]
    private async Task AbrirDetalhesSolicitacaoAsync(SolicitacaoModel itemSelecionado)
    {
        if (itemSelecionado == null) return;

        var solicitacao = _solicitacoesOriginais.FirstOrDefault(s =>
            s.Status == itemSelecionado.NomeStatus &&
            s.Itens.Any(i => i.Sku == itemSelecionado.Sku));

        if (solicitacao != null)
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