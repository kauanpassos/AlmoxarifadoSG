using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.Domain.Constants;
using Almoxarifado.Application.DTOs;
using Microsoft.Maui.Controls;

namespace Almoxarifado.App.ViewModels;

[QueryProperty(nameof(SolicitacaoSelecionada), "SolicitacaoSelecionada")]
public partial class DetalheSolicitacaoViewModel : ObservableObject
{
    private readonly IFirebaseService _firebaseService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PodeCancelar))]
    private SolicitacaoDto? _solicitacaoSelecionada;

    [ObservableProperty]
    private ObservableCollection<ItemSolicitacaoDto> _itens = new();

    [ObservableProperty]
    private string _nomeSolicitante = "Carregando...";

    public bool PodeCancelar => SolicitacaoSelecionada?.Status == StatusSolicitacao.Pendente
                             || SolicitacaoSelecionada?.Status == "Em análise";

    public DetalheSolicitacaoViewModel(IFirebaseService firebaseService, IDialogService dialogService)
    {
        _firebaseService = firebaseService;
        _dialogService = dialogService;
    }

    partial void OnSolicitacaoSelecionadaChanged(SolicitacaoDto? value)
    {
        if (value != null)
        {
            CarregarItens(value);

            _ = CarregarNomeUsuarioAsync(value.UsuarioId);
        }
    }

    private async Task CarregarNomeUsuarioAsync(string usuarioId)
    {
        try
        {
            var usuario = await _firebaseService.GetUsuarioAsync(usuarioId);
            NomeSolicitante = usuario?.Nome ?? "Usuário Desconhecido";
        }
        catch (Exception)
        {
            NomeSolicitante = "Não foi possível carregar o nome";
        }
    }

    private void CarregarItens(SolicitacaoDto solicitacao)
    {
        Itens.Clear();

        if (solicitacao.Itens != null)
        {
            foreach (var item in solicitacao.Itens)
            {
                Itens.Add(item);
            }
        }
    }

    [RelayCommand]
    private async Task VoltarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task CancelarSolicitacao()
    {
        if (SolicitacaoSelecionada == null) return;

        bool confirmacao = await _dialogService.ShowConfirmationAsync(
            "Cancelar Solicitação",
            "Tem a certeza que deseja cancelar esta solicitação?\nEsta ação não poderá ser desfeita.",
            "Sim, Cancelar",
            "Voltar");

        if (confirmacao)
        {
            try
            {
                await _firebaseService.AtualizarStatusSolicitacaoAsync(SolicitacaoSelecionada.Id, StatusSolicitacao.Cancelada);

                SolicitacaoSelecionada = SolicitacaoSelecionada with { Status = StatusSolicitacao.Cancelada };

                OnPropertyChanged(nameof(SolicitacaoSelecionada));
                OnPropertyChanged(nameof(PodeCancelar));

                await _dialogService.ShowAlertAsync("Sucesso", "A solicitação foi cancelada com sucesso.", "OK");

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync("Erro", $"Não foi possível cancelar: {ex.Message}", "OK");
            }
        }
    }
}