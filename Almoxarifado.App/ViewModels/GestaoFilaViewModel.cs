using Almoxarifado.App.Services;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.Application.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Almoxarifado.App.ViewModels;

public sealed partial class GestaoFilaViewModel : ObservableObject
{
    private readonly IFirebaseService _firebaseService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;

    public ObservableCollection<SolicitacaoDto> Solicitacoes { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    private bool _isBusy;

    public bool IsEmpty => Solicitacoes.Count is 0 && IsBusy is false;

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
        ArgumentNullException.ThrowIfNull(firebaseService);
        ArgumentNullException.ThrowIfNull(navigationService);
        ArgumentNullException.ThrowIfNull(dialogService);

        _firebaseService = firebaseService;
        _navigationService = navigationService;
        _dialogService = dialogService;
    }

    [RelayCommand]
    private async Task IrParaPerfilAsync()
    {
        await _navigationService.NavigateToAsync("//PerfilPage");
    }

    [RelayCommand]
    private async Task IrParaEstoqueAsync()
    {
        await Shell.Current.GoToAsync("EstoquePage");
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            Solicitacoes.Clear();

            if (UsuarioSessao.UsuarioLogado is null) return;

            var dados = await _firebaseService.GetSolicitacoesPendentesAsync();

            foreach (var item in dados)
            {
                Solicitacoes.Add(item);
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

    [RelayCommand]
    private async Task AprovarAsync(SolicitacaoDto solicitacao)
    {
        await AlterarStatusAsync(solicitacao, "Aprovada");
    }

    [RelayCommand]
    private async Task RecusarAsync(SolicitacaoDto solicitacao)
    {
        await AlterarStatusAsync(solicitacao, "Recusada");
    }

    private async Task AlterarStatusAsync(SolicitacaoDto solicitacao, string novoStatus)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            
            await _firebaseService.AtualizarStatusSolicitacaoAsync(solicitacao.Id, novoStatus);
            await LoadAsync();
        }
        catch (Exception)
        {
            await _dialogService.ShowAlertAsync("Erro na Atualização", "Houve uma falha inesperada ao tentar atualizar o status da solicitação.");
        }
        finally
        {
            IsBusy = false;
        }
    }
}