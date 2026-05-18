using Almoxarifado.App.Services;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.App.Views;
using Almoxarifado.Domain;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Almoxarifado.App.ViewModels;

public partial class NovaSolicitacaoViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string _iniciaisUsuario = string.Empty;

    [ObservableProperty]
    private int _quantidadeSolicitada = 1;

    [ObservableProperty]
    private int _estoqueDisponivel;

    [ObservableProperty]
    private ObservableCollection<string> _sugestoesRapidas = new();

    public NovaSolicitacaoViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

        var usuarioLogado = UsuarioSessao.UsuarioLogado;
        if (usuarioLogado != null)
        {
            IniciaisUsuario = ObterIniciais(usuarioLogado.Nome);
        }
    }

    [RelayCommand]
    private void AumentarQuantidade() => QuantidadeSolicitada++;

    [RelayCommand]
    private void DiminuirQuantidade()
    {
        if (QuantidadeSolicitada > 1) QuantidadeSolicitada--;
    }

    [RelayCommand]
    private async Task VoltarAsync()
    {
        var usuarioLogado = UsuarioSessao.UsuarioLogado;
        if (usuarioLogado?.Tipo == "Almoxarife")
            await _navigationService.NavigateToAsync("//GestaoFilaPage");
        else
            await _navigationService.NavigateToAsync("//HomeColaboradorPage");
    }

    [RelayCommand]
    private async Task IrParaPerfilAsync()
    {
        await _navigationService.NavigateToAsync(nameof(PerfilPage));
    }

    [RelayCommand]
    private async Task ConfirmarPedidoAsync()
    {
        await Shell.Current.DisplayAlert("Sucesso", $"Solicitação de {QuantidadeSolicitada} unidade(s) confirmada!", "OK");
        await VoltarAsync();
    }

    private string ObterIniciais(string? nome)
    {
        if (string.IsNullOrWhiteSpace(nome)) return "US";
        var partes = nome.Trim().Split(' ');
        if (partes.Length == 1) return partes[0].Substring(0, Math.Min(2, partes[0].Length)).ToUpper();
        return $"{partes[0][0]}{partes[^1][0]}".ToUpper();
    }
}