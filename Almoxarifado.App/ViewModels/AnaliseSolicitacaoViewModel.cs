using Almoxarifado.App.Models;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.Application.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Almoxarifado.App.ViewModels;

[QueryProperty(nameof(SolicitacaoSelecionada), "SolicitacaoSelecionada")]
public sealed partial class AnaliseSolicitacaoViewModel : ObservableObject
{
    private const string EstadoAguardando = "Aguardando";
    private const string EstadoValidado = "Validado";
    private const string EstadoEmFalta = "EmFalta";

    private const string StatusAprovada = "Aprovada";
    private const string StatusRecusada = "Recusada";
    private const string StatusProdutoSemNome = "Produto Sem Nome";

    private readonly IFirebaseService _firebaseService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _podeFinalizar;

    private SolicitacaoDto? _solicitacaoSelecionada;
    public SolicitacaoDto? SolicitacaoSelecionada
    {
        get => _solicitacaoSelecionada;
        set
        {
            SetProperty(ref _solicitacaoSelecionada, value);
            if (value != null) CarregarItens(value);
        }
    }

    public ObservableCollection<ItemAnaliseModel> ItensAnalise { get; } = new();

    public AnaliseSolicitacaoViewModel(IFirebaseService firebaseService, IDialogService dialogService)
    {
        _firebaseService = firebaseService ?? throw new ArgumentNullException(nameof(firebaseService));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
    }

    private void CarregarItens(SolicitacaoDto solicitacao)
    {
        ItensAnalise.Clear();
        foreach (var item in solicitacao.Itens)
        {
            var modelo = new ItemAnaliseModel
            {
                Sku = item.Sku,
                NomeProduto = string.IsNullOrWhiteSpace(item.NomeProduto) ? StatusProdutoSemNome : item.NomeProduto,
                Quantidade = item.Quantidade,
                AoAlterarEstado = VerificarConclusao
            };
            ItensAnalise.Add(modelo);
        }
        VerificarConclusao();
    }

    private void VerificarConclusao()
    {
        PodeFinalizar = ItensAnalise.Count > 0 && ItensAnalise.All(i => i.Estado != EstadoAguardando);
    }

    [RelayCommand]
    private async Task VoltarAsync() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private async Task FinalizarAnaliseAsync()
    {
        if (SolicitacaoSelecionada == null || IsBusy) return;

        var possuiItemEmFalta = ItensAnalise.Any(i => i.Estado == EstadoEmFalta);
        var statusFinal = possuiItemEmFalta ? StatusRecusada : StatusAprovada;

        string mensagemConfirmacao = possuiItemEmFalta
            ? "Atenção: Existem itens marcados 'Em Falta'. Esta solicitação será INTEGRALMENTE RECUSADA. Confirma?"
            : "Todos os itens foram validados com sucesso. Deseja APROVAR a solicitação?";

        var confirma = await Microsoft.Maui.Controls.Application.Current!.MainPage!.DisplayAlert(
            "Confirmar Análise",
            mensagemConfirmacao,
            "Confirmar", "Cancelar");

        if (!confirma) return;

        try
        {
            IsBusy = true;
            await _firebaseService.AtualizarStatusSolicitacaoAsync(SolicitacaoSelecionada.Id, statusFinal);

            string msgSucesso = statusFinal == StatusAprovada
                ? "Solicitação aprovada e estoque atualizado!"
                : "Solicitação recusada com sucesso.";

            await _dialogService.ShowAlertAsync("Sucesso", msgSucesso);
            await VoltarAsync();
        }
        catch (Exception)
        {
            await _dialogService.ShowAlertAsync("Erro", "Falha ao processar a solicitação no servidor. Verifique o estoque e tente novamente.");
        }
        finally
        {
            IsBusy = false;
        }
    }
}