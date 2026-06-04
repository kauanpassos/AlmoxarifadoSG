using Almoxarifado.App.Services;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.App.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;

namespace Almoxarifado.App.ViewModels;

public sealed partial class CheckoutViewModel : ObservableObject
{
    private readonly ICartService _cartService;
    private readonly IFirebaseService _firebaseService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string _justificativa = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    public bool IsNotBusy => IsBusy is false;

    public ObservableCollection<CartItemModel> Itens => _cartService.Itens;

    public int TotalItens => _cartService.TotalItens;

    public CheckoutViewModel(
        ICartService cartService, 
        IFirebaseService firebaseService, 
        INavigationService navigationService,
        IDialogService dialogService,
        IAuthService authService)
    {
        ArgumentNullException.ThrowIfNull(cartService);
        ArgumentNullException.ThrowIfNull(firebaseService);
        ArgumentNullException.ThrowIfNull(navigationService);
        ArgumentNullException.ThrowIfNull(dialogService);
        ArgumentNullException.ThrowIfNull(authService);

        _cartService = cartService;
        _firebaseService = firebaseService;
        _navigationService = navigationService;
        _dialogService = dialogService;
        _authService = authService;
    }

    [RelayCommand]
    private async Task FinalizarPedidoAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Justificativa))
        {
            await _dialogService.ShowAlertAsync("Aviso", "Por favor, insira uma justificativa para o pedido.");
            return;
        }

        if (Itens.Count is 0)
        {
            await _dialogService.ShowAlertAsync("Aviso", "O carrinho de solicitações está vazio.");
            return;
        }

        IsBusy = true;

        try
        {
            var usuarioId = _authService.ObterUsuarioIdAtual();
            
            if (usuarioId is null)
            {
                await _dialogService.ShowAlertAsync("Erro de Autenticação", "Não foi possível identificar a sessão do usuário ativo.");
                return;
            }

            var itensDto = Itens.Select(i => i.ToDto()).ToList();

            await _firebaseService.EnviarSolicitacaoLoteAsync(usuarioId, Justificativa, itensDto);
            
            _cartService.LimparCarrinho();
            
            await _dialogService.ShowAlertAsync("Sucesso", "Pedido enviado para aprovação com sucesso!");
            
            await _navigationService.NavigateToHomeAsync();
        }
        catch (Exception)
        {
            await _dialogService.ShowAlertAsync("Falha de Processamento", "Ocorreu um erro inesperado ao tentar processar e enviar o seu lote de solicitações.");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
