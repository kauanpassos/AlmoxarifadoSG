using Almoxarifado.App.Extensions;
using Almoxarifado.App.Services;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.Application.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Almoxarifado.App.ViewModels;

public partial class CheckoutViewModel : ObservableObject
{
    private readonly IFirebaseService _firebaseService;
    private readonly ICartService _cartService;
    private readonly IAuthService _authService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsListaVazia))]
    [NotifyPropertyChangedFor(nameof(IsListaComItens))]
    private ObservableCollection<CartItemModel> _materiaisSolicitados = new();

    [ObservableProperty]
    private string _justificativa = string.Empty;

    public bool IsListaVazia => MateriaisSolicitados.Count == 0;
    public bool IsListaComItens => MateriaisSolicitados.Count > 0;

    public CheckoutViewModel(IFirebaseService firebaseService, ICartService cartService, IAuthService authService, IDialogService dialogService)
    {
        _firebaseService = firebaseService;
        _cartService = cartService;
        _authService = authService;
        _dialogService = dialogService;

        CarregarMateriais();
    }

    private void CarregarMateriais()
    {
        MateriaisSolicitados = new ObservableCollection<CartItemModel>(_cartService.Itens);
    }

    [RelayCommand]
    private void AumentarQuantidade(CartItemModel item)
    {
        if (item == null) return;

        if (item.Quantidade >= 999)
        {
            _dialogService.ShowAlertAsync("Aviso", "Quantidade máxima atingida para este material.", "OK");
            return;
        }

        item.Quantidade++;
        var index = MateriaisSolicitados.IndexOf(item);
        if (index >= 0) MateriaisSolicitados[index] = item;
    }

    [RelayCommand]
    private void DiminuirQuantidade(CartItemModel item)
    {
        if (item == null) return;

        if (item.Quantidade > 1)
        {
            item.Quantidade--;
            var index = MateriaisSolicitados.IndexOf(item);
            if (index >= 0) MateriaisSolicitados[index] = item;
        }
    }

    [RelayCommand]
    private async Task RemoverItem(CartItemModel item)
    {
        if (item == null) return;

        string nomeProduto = !string.IsNullOrWhiteSpace(item.Produto.NomePeca) ? item.Produto.NomePeca : item.Produto.Sku;

        bool confirmar = await _dialogService.ShowConfirmationAsync("Remover Material", $"Deseja remover o material '{nomeProduto}' da solicitação?", "Sim, Remover", "Cancelar");
        if (confirmar)
        {
            MateriaisSolicitados.Remove(item);
            _cartService.RemoverItem(item.Produto.Sku);

            OnPropertyChanged(nameof(IsListaVazia));
            OnPropertyChanged(nameof(IsListaComItens));
        }
    }

    [RelayCommand]
    private async Task EnviarSolicitacao()
    {
        if (IsListaVazia) return;

        if (string.IsNullOrWhiteSpace(Justificativa))
        {
            await _dialogService.ShowAlertAsync("Atenção", "A justificativa da solicitação é obrigatória. Por favor, informe o motivo do pedido.", "OK");
            return;
        }

        try
        {
            var usuarioId = _authService.ObterUsuarioIdAtual();
            if (string.IsNullOrEmpty(usuarioId))
            {
                await _dialogService.ShowAlertAsync("Erro", "A sessão expirou. Faça o login novamente.", "OK");
                return;
            }

            var itensParaEnviar = MateriaisSolicitados.Select(i => i.ToDto()).ToList();

            await _firebaseService.EnviarSolicitacaoLoteAsync(usuarioId, Justificativa, itensParaEnviar);

            _cartService.LimparCarrinho();
            MateriaisSolicitados.Clear();
            Justificativa = string.Empty;

            await _dialogService.ShowAlertAsync("Sucesso", "Solicitação enviada com sucesso!", "OK");

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAlertAsync("Erro na Solicitação", ex.Message, "OK");
        }
    }
}