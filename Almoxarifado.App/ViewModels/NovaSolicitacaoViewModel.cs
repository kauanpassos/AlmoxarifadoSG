using Almoxarifado.App.Services;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.App.Views;
using Almoxarifado.Domain;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Storage;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Almoxarifado.App.ViewModels;

public partial class NovaSolicitacaoViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly IFirebaseService _firebaseService;
    private ObservableCollection<Produto> _todosProdutos = new();

    [ObservableProperty]
    private string _iniciaisUsuario = string.Empty;

    [ObservableProperty]
    private int _quantidadeSolicitada = 1;

    [ObservableProperty]
    private int _estoqueDisponivel;

    [ObservableProperty]
    private string _termoBusca = string.Empty;

    [ObservableProperty]
    private string _justificativa = string.Empty;

    [ObservableProperty]
    private Produto? _pecaSelecionada;

    [ObservableProperty]
    private bool _isBusy;

    public ObservableCollection<Produto> ProdutosFiltrados { get; } = new();

    public NovaSolicitacaoViewModel(INavigationService navigationService, IFirebaseService firebaseService)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _firebaseService = firebaseService ?? throw new ArgumentNullException(nameof(firebaseService));

        var usuarioLogado = UsuarioSessao.UsuarioLogado;
        if (usuarioLogado != null)
            IniciaisUsuario = ObterIniciais(usuarioLogado.Nome);
    }

    partial void OnTermoBuscaChanged(string value) => AplicarFiltroProdutos(value);

    partial void OnPecaSelecionadaChanged(Produto? value)
    {
        EstoqueDisponivel = 0;
    }

    [RelayCommand]
    private async Task CarregarProdutosAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            var produtos = await _firebaseService.GetProdutosAsync();
            _todosProdutos = new ObservableCollection<Produto>(produtos.Where(p => p.Ativo));
            AplicarFiltroProdutos(TermoBusca);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erro", $"Não foi possível carregar os produtos: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
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
        var usuario = UsuarioSessao.UsuarioLogado;
        if (usuario == null)
        {
            await Shell.Current.DisplayAlert("Sessão expirada", "Faça login novamente.", "OK");
            return;
        }

        if (PecaSelecionada == null)
        {
            await Shell.Current.DisplayAlert("Atenção", "Selecione uma peça antes de confirmar.", "OK");
            return;
        }

        if (QuantidadeSolicitada <= 0)
        {
            await Shell.Current.DisplayAlert("Atenção", "A quantidade deve ser maior que zero.", "OK");
            return;
        }

        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var idToken = await SecureStorage.Default.GetAsync("auth_token") ?? string.Empty;
            await _firebaseService.EnviarSolicitacaoAsync(
                usuarioId: usuario.Id,
                sku: PecaSelecionada.Sku,
                quantidade: QuantidadeSolicitada,
                justificativa: string.IsNullOrWhiteSpace(Justificativa) ? "Sem justificativa informada." : Justificativa,
                idToken: idToken
            );
            await Shell.Current.DisplayAlert("Solicitação enviada", $"Pedido de {QuantidadeSolicitada} un de '{PecaSelecionada.Nome}' registrado com sucesso.", "OK");
            await VoltarAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erro", $"Não foi possível registrar a solicitação: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void AplicarFiltroProdutos(string termo)
    {
        ProdutosFiltrados.Clear();
        var filtrados = string.IsNullOrWhiteSpace(termo)
            ? _todosProdutos
            : _todosProdutos.Where(p =>
                p.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                p.Sku.Contains(termo, StringComparison.OrdinalIgnoreCase));
        foreach (var p in filtrados)
            ProdutosFiltrados.Add(p);
    }

    private string ObterIniciais(string? nome)
    {
        if (string.IsNullOrWhiteSpace(nome)) return "US";
        var partes = nome.Trim().Split(' ');
        if (partes.Length == 1) return partes[0].Substring(0, Math.Min(2, partes[0].Length)).ToUpper();
        return $"{partes[0][0]}{partes[^1][0]}".ToUpper();
    }
}