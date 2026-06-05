using Almoxarifado.App.Models;
using Almoxarifado.App.Services;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.App.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Almoxarifado.App.ViewModels;

public partial class HomeColaboradorViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly IFirebaseService _firebaseService;

    [ObservableProperty]
    private string _iniciaisUsuario = string.Empty;

    [ObservableProperty]
    private string _termoPesquisa = string.Empty;

    public ObservableCollection<SolicitacaoModel> ListaSolicitacoes { get; } = new();

    public HomeColaboradorViewModel(INavigationService navigationService, IFirebaseService firebaseService)
    {
        ArgumentNullException.ThrowIfNull(navigationService);
        ArgumentNullException.ThrowIfNull(firebaseService);

        _navigationService = navigationService;
        _firebaseService = firebaseService;

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
            var usuario = UsuarioSessao.UsuarioLogado;
            
            if (usuario is null) return;

            var solicitacoes = await _firebaseService.GetSolicitacoesUsuarioAsync(usuario.Id);
            
            var modelos = solicitacoes.SelectMany(s => s.Itens.Select(i => new SolicitacaoModel
            {
                NomePeca = i.Sku,
                Sku = i.Sku,
                Quantidade = i.Quantidade,
                NomeStatus = s.Status
            }));

            foreach (var model in modelos)
            {
                ListaSolicitacoes.Add(model);
            }
        }
        catch (Exception)
        {
            if (Microsoft.Maui.Controls.Application.Current?.MainPage is not null)
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Erro", "Não foi possível carregar o dashboard via Firebase.", "OK");
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