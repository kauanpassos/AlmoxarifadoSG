using Almoxarifado.App.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;

namespace Almoxarifado.App.Services;

public class CartItemModel
{
    public ItemEstoqueModel Produto { get; set; } = new();
    public int Quantidade { get; set; }
}

public interface ICartService
{
    ObservableCollection<CartItemModel> Itens { get; }
    int TotalItens { get; }
    void AdicionarItem(ItemEstoqueModel produto, int quantidade);
    void RemoverItem(string sku);
    void LimparCarrinho();
    event Action? OnCarrinhoAtualizado;
}

public class CartService : ICartService
{
    public ObservableCollection<CartItemModel> Itens { get; } = new();

    public int TotalItens => Itens.Sum(i => i.Quantidade);

    public event Action? OnCarrinhoAtualizado;

    public void AdicionarItem(ItemEstoqueModel produto, int quantidade)
    {
        var itemExistente = Itens.FirstOrDefault(i => i.Produto.Sku == produto.Sku);

        if (itemExistente != null)
        {
            itemExistente.Quantidade += quantidade;
        }
        else
        {
            Itens.Add(new CartItemModel { Produto = produto, Quantidade = quantidade });
        }

        OnCarrinhoAtualizado?.Invoke();
    }

    public void RemoverItem(string sku)
    {
        var item = Itens.FirstOrDefault(i => i.Produto.Sku == sku);
        if (item != null)
        {
            Itens.Remove(item);
            OnCarrinhoAtualizado?.Invoke();
        }
    }

    public void LimparCarrinho()
    {
        Itens.Clear();
        OnCarrinhoAtualizado?.Invoke();
    }
}