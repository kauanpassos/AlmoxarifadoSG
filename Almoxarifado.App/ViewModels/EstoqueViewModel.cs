using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;

namespace Almoxarifado.App.ViewModels;

// Melhorei com defaults para JSON
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

// As ViewModel estão limpas e preparadas para API
public partial class EstoqueViewModel : ObservableObject
{
    [ObservableProperty]
    private string _termoPesquisa = string.Empty;

    public ObservableCollection<ItemEstoqueModel> PecasEstoque { get; } = new();
    public EstoqueViewModel()
    {
    }

    [RelayCommand]
    private void ExecuteFiltrarTodos()
    {
        PecasEstoque.Clear();
    }

    [RelayCommand]
    private void ExecuteFiltrarBaixoEstoque()
    {
        PecasEstoque.Clear();
    }

    [RelayCommand]
    private void ExecuteFiltrarIndisponivel()
    {
        PecasEstoque.Clear();
    }
}