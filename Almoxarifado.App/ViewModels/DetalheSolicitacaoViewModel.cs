using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Almoxarifado.App.Services.Interfaces;
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
    private SolicitacaoDto _solicitacaoSelecionada;

    [ObservableProperty]
    private ObservableCollection<ItemSolicitacaoDto> _itens = new();

    // Nova propriedade para guardar o nome real que vem da API
    [ObservableProperty]
    private string _nomeSolicitante = "Carregando...";

    public bool PodeCancelar => SolicitacaoSelecionada?.Status == "Pendente" || SolicitacaoSelecionada?.Status == "Em análise";

    public DetalheSolicitacaoViewModel(IFirebaseService firebaseService, IDialogService dialogService)
    {
        _firebaseService = firebaseService;
        _dialogService = dialogService;
    }

    partial void OnSolicitacaoSelecionadaChanged(SolicitacaoDto value)
    {
        if (value != null)
        {
            CarregarItens(value);

            // Dispara a busca do nome do usuário em segundo plano
            _ = CarregarNomeUsuarioAsync(value.UsuarioId);
        }
    }

    private async Task CarregarNomeUsuarioAsync(string usuarioId)
    {
        try
        {
            // Vai na API buscar o perfil e pega o nome
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

    // 🔥 NOVO: Comando para voltar para a tela anterior
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
            $"Tem a certeza que deseja cancelar esta solicitação?\nEsta ação não poderá ser desfeita.",
            "Sim, Cancelar",
            "Voltar");

        if (confirmacao)
        {
            try
            {
                // Chama a API para cancelar no banco de dados
                await _firebaseService.AtualizarStatusSolicitacaoAsync(SolicitacaoSelecionada.Id, "Cancelado");

                // Atualiza na tela local usando 'with' para contornar o 'record' imutável
                SolicitacaoSelecionada = SolicitacaoSelecionada with { Status = "Cancelado" };

                OnPropertyChanged(nameof(SolicitacaoSelecionada));
                OnPropertyChanged(nameof(PodeCancelar));

                await _dialogService.ShowAlertAsync("Sucesso", "A solicitação foi cancelada com sucesso.", "OK");

                // Volta para o Dashboard
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync("Erro", $"Não foi possível cancelar: {ex.Message}", "OK");
            }
        }
    }
}