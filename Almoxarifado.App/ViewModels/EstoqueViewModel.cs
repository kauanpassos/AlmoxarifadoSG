using Almoxarifado.App.Popups;
using Almoxarifado.App.Services;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.App.Views;
using Almoxarifado.Domain.Enums;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Almoxarifado.App.ViewModels;

public class ItemEstoqueModel
{
    public string Id { get; set; } = string.Empty;
    public string NomePeca { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public long Quantidade { get; set; }
    public string DataAtualizacao { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public partial class EstoqueViewModel : ObservableObject
{
    private readonly IFirebaseService _firebaseService;
    private readonly INavigationService _navigationService;
    private readonly ICartService _cartService;
    private readonly List<ItemEstoqueModel> _todasAsPecasCache = new();

    private bool _estaCarregando;
    public bool EstaCarregando
    {
        get => _estaCarregando;
        set => SetProperty(ref _estaCarregando, value);
    }

    private string _termoPesquisa = string.Empty;
    public string TermoPesquisa
    {
        get => _termoPesquisa;
        set
        {
            // SetProperty atualiza o valor. Se for alterado, executa a pesquisa
            if (SetProperty(ref _termoPesquisa, value))
            {
                ExecutarPesquisa();
            }
        }
    }

    private string _iniciaisUsuario = string.Empty;
    public string IniciaisUsuario
    {
        get => _iniciaisUsuario;
        set => SetProperty(ref _iniciaisUsuario, value);
    }

    private int _quantidadeCarrinho;
    public int QuantidadeCarrinho
    {
        get => _quantidadeCarrinho;
        set => SetProperty(ref _quantidadeCarrinho, value);
    }

    private bool _temItensNoCarrinho;
    public bool TemItensNoCarrinho
    {
        get => _temItensNoCarrinho;
        set => SetProperty(ref _temItensNoCarrinho, value);
    }

    public ObservableCollection<ItemEstoqueModel> PecasEstoque { get; } = new();

    public EstoqueViewModel(IFirebaseService firebaseService, INavigationService navigationService, ICartService cartService)
    {
        _firebaseService = firebaseService ?? throw new ArgumentNullException(nameof(firebaseService));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));

        _cartService.OnCarrinhoAtualizado += AtualizarStatusCarrinho;
        AtualizarStatusCarrinho();

        var usuarioLogado = UsuarioSessao.UsuarioLogado;
        if (usuarioLogado != null)
        {
            IniciaisUsuario = ObterIniciais(usuarioLogado.Nome);
        }

        // Dispara a busca de dados assim que a tela é instanciada
        _ = CarregarEstoqueAsync();
    }

    private void AtualizarStatusCarrinho()
    {
        QuantidadeCarrinho = _cartService.TotalItens;
        TemItensNoCarrinho = QuantidadeCarrinho > 0;
    }

    [RelayCommand]
    private async Task AbrirOpcoesProdutoAsync(ItemEstoqueModel produtoSelecionado)
    {
        if (produtoSelecionado == null) return;

        if (produtoSelecionado.Quantidade == 0)
        {
            await Shell.Current.DisplayAlert("Aviso", "Este item está indisponível no momento.", "OK");
            return;
        }

        var popup = new QuantidadePopup(produtoSelecionado.NomePeca, produtoSelecionado.Quantidade);
        var resultado = await Shell.Current.ShowPopupAsync(popup);

        if (resultado is int qtdSelecionada && qtdSelecionada > 0)
        {
            _cartService.AdicionarItem(produtoSelecionado, qtdSelecionada);
        }
    }

    [RelayCommand]
    private async Task IrParaCarrinhoAsync()
    {
        await Shell.Current.DisplayAlert("Carrinho", $"Você tem {QuantidadeCarrinho} itens. Em breve o ecrã de checkout!", "OK");
    }

    [RelayCommand]
    public async Task CarregarEstoqueAsync()
    {
        if (EstaCarregando) return;

        try
        {
            EstaCarregando = true;
            _todasAsPecasCache.Clear();

            // Busca os dados reais da API/Firestore
            var pecasDoBanco = await _firebaseService.GetProdutosAsync();

            if (pecasDoBanco != null && pecasDoBanco.Any())
            {
                foreach (var peca in pecasDoBanco)
                {
                    var item = new ItemEstoqueModel
                    {
                        Id = peca.Id,
                        NomePeca = peca.Nome,
                        Sku = peca.NumCode.ToString(),
                        Quantidade = peca.QtdEstoque,
                        DataAtualizacao = peca.UpdatedAt.ToString("dd/MM/yyyy HH:mm")
                    };

                    if (peca.QtdEstoque <= 0)
                        item.Status = "Indisponível";
                    else if (peca.QtdEstoque <= peca.EstoqueMinimo)
                        item.Status = "Baixo Estoque";
                    else
                        item.Status = "Disponível";

                    _todasAsPecasCache.Add(item);
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Aviso", "Nenhum produto encontrado no banco de dados.", "OK");
            }

            FiltrarTodos();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar estoque: {ex.Message}");
            await Shell.Current.DisplayAlert("Erro de Conexão", "Não foi possível acessar a API do banco de dados.", "OK");
        }
        finally
        {
            EstaCarregando = false;
        }
    }

    // LÓGICA DE PESQUISA (NOME OU NUMCODE)
    private void ExecutarPesquisa()
    {
        if (string.IsNullOrWhiteSpace(TermoPesquisa))
        {
            FiltrarTodos();
            return;
        }

        var termo = TermoPesquisa.ToLowerInvariant();

        PecasEstoque.Clear();
        var filtrados = _todasAsPecasCache.Where(p =>
            p.NomePeca.ToLowerInvariant().Contains(termo) ||
            p.Sku.Contains(termo)
        );

        foreach (var item in filtrados)
        {
            PecasEstoque.Add(item);
        }
    }

    [RelayCommand]
    private void FiltrarTodos()
    {
        PecasEstoque.Clear();
        foreach (var item in _todasAsPecasCache)
        {
            PecasEstoque.Add(item);
        }
    }

    [RelayCommand]
    private void FiltrarIndisponivel()
    {
        PecasEstoque.Clear();
        var filtrados = _todasAsPecasCache.Where(p => p.Status == "Indisponível");
        foreach (var item in filtrados)
        {
            PecasEstoque.Add(item);
        }
    }

    [RelayCommand]
    private async Task VoltarAsync()
    {
        var usuarioLogado = UsuarioSessao.UsuarioLogado;
        if (usuarioLogado?.Tipo == TipoUsuario.Almoxarife)
            await _navigationService.NavigateToAsync("//GestaoFilaPage");
        else
            await _navigationService.NavigateToAsync("//HomeColaboradorPage");
    }

    [RelayCommand]
    private async Task IrParaPerfilAsync() => await _navigationService.NavigateToAsync(nameof(PerfilPage));

    private string ObterIniciais(string? nome)
    {
        if (string.IsNullOrWhiteSpace(nome)) return "US";
        var partes = nome.Trim().Split(' ');
        if (partes.Length == 1) return partes[0].Substring(0, Math.Min(2, partes[0].Length)).ToUpper();
        return $"{partes[0][0]}{partes[^1][0]}".ToUpper();
    }
}