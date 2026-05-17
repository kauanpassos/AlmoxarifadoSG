using Almoxarifado.Application.Queries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

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

    private readonly List<ItemEstoqueModel> _todasAsPecasCache = new();

    private bool _estaCarregando;

    [ObservableProperty]
    private string _termoPesquisa = string.Empty;

    public ObservableCollection<ItemEstoqueModel> PecasEstoque { get; } = new();
    public EstoqueViewModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    [RelayCommand]
    public async Task CarregarEstoqueAsync()
    {
        if (_estaCarregando)
            return;

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
        foreach (var item in _todasAsPecasCache)
            PecasEstoque.Add(item);
    }

    [RelayCommand]
    private void ExecuteFiltrarBaixoEstoque()
    {
        PecasEstoque.Clear();
        var filtrados = _todasAsPecasCache.Where(p => p.Quantidade > 0 && p.Quantidade <= 20);
        foreach (var item in filtrados)
            PecasEstoque.Add(item);
    }

    [RelayCommand]
    private void ExecuteFiltrarIndisponivel()
    {
        PecasEstoque.Clear();
        var filtrados = _todasAsPecasCache.Where(p => p.Quantidade == 0);
        foreach (var item in filtrados)
            PecasEstoque.Add(item);
    }
}