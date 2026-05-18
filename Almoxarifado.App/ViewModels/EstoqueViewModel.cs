using Almoxarifado.App.Services;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.App.Views;
using Almoxarifado.Application.Queries;
using Almoxarifado.Domain;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Almoxarifado.App.ViewModels;

public class ItemEstoqueModel
{
    public string Id { get; set; } = string.Empty;
    public string NomePeca { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public string DataAtualizacao { get; set; } = string.Empty;
    public string TextoStatus { get; set; } = string.Empty;
    public string CorFundoStatus { get; set; } = "Transparent";
    public string CorBordaStatus { get; set; } = "Transparent";
    public string CorTextoStatus { get; set; } = "Black";
}

public partial class EstoqueViewModel : ObservableObject
{
    private readonly IMediator _mediator;
    private readonly INavigationService _navigationService;
    private readonly List<ItemEstoqueModel> _todasAsPecasCache = new();
    private bool _estaCarregando;

    [ObservableProperty]
    private string _termoPesquisa = string.Empty;

    [ObservableProperty]
    private string _iniciaisUsuario = string.Empty;

    public ObservableCollection<ItemEstoqueModel> PecasEstoque { get; } = new();

    public EstoqueViewModel(IMediator mediator, INavigationService navigationService)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

        var usuarioLogado = UsuarioSessao.UsuarioLogado;
        if (usuarioLogado != null)
        {
            IniciaisUsuario = ObterIniciais(usuarioLogado.Nome);
        }
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
    public async Task CarregarEstoqueAsync()
    {
        if (_estaCarregando) return;
        if (_todasAsPecasCache.Any())
        {
            ExecuteFiltrarTodos();
            return;
        }

        try
        {
            _estaCarregando = true;
            var pecasDoBanco = await _mediator.Send(new GetEstoqueQuery());
            _todasAsPecasCache.Clear();

            foreach (var peca in pecasDoBanco)
            {
                var item = new ItemEstoqueModel
                {
                    Id = peca.Id.ToString(),
                    NomePeca = peca.NomePeca,
                    Sku = peca.DescricaoTecnica,
                    Quantidade = peca.Quantidade,
                    DataAtualizacao = DateTime.Now.ToString("dd/MM/yyyy")
                };

                if (peca.Quantidade == 0)
                {
                    item.TextoStatus = "Indisponível";
                    item.CorFundoStatus = "#FBE6E6";
                    item.CorBordaStatus = "#DA0004";
                    item.CorTextoStatus = "#DA0004";
                }
                else if (peca.Quantidade <= 20)
                {
                    item.TextoStatus = "Baixo Estoque";
                    item.CorFundoStatus = "#FCFDEA";
                    item.CorBordaStatus = "#DFE92B";
                    item.CorTextoStatus = "#9A9A00";
                }
                else
                {
                    item.TextoStatus = "Normal";
                    item.CorFundoStatus = "#E6F8E8";
                    item.CorBordaStatus = "#03BA15";
                    item.CorTextoStatus = "#03BA15";
                }

                _todasAsPecasCache.Add(item);
            }
            ExecuteFiltrarTodos();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar estoque: {ex.Message}");
        }
        finally
        {
            _estaCarregando = false;
        }
    }

    [RelayCommand]
    private void ExecuteFiltrarTodos()
    {
        PecasEstoque.Clear();
        foreach (var item in _todasAsPecasCache) PecasEstoque.Add(item);
    }

    [RelayCommand]
    private void ExecuteFiltrarBaixoEstoque()
    {
        PecasEstoque.Clear();
        var filtrados = _todasAsPecasCache.Where(p => p.Quantidade > 0 && p.Quantidade <= 20);
        foreach (var item in filtrados) PecasEstoque.Add(item);
    }

    [RelayCommand]
    private void ExecuteFiltrarIndisponivel()
    {
        PecasEstoque.Clear();
        var filtrados = _todasAsPecasCache.Where(p => p.Quantidade == 0);
        foreach (var item in filtrados) PecasEstoque.Add(item);
    }

    private string ObterIniciais(string? nome)
    {
        if (string.IsNullOrWhiteSpace(nome)) return "US";
        var partes = nome.Trim().Split(' ');
        if (partes.Length == 1) return partes[0].Substring(0, Math.Min(2, partes[0].Length)).ToUpper();
        return $"{partes[0][0]}{partes[^1][0]}".ToUpper();
    }
}