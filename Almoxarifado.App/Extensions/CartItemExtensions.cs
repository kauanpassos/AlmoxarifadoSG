using Almoxarifado.App.Services;
using Almoxarifado.Application.DTOs;

namespace Almoxarifado.App.Extensions;

public static class CartItemExtensions
{
    public static CartItemDto ToDto(this CartItemModel model)
    {
        return new CartItemDto(model.Produto.Sku, model.Quantidade, model.Produto.NomePeca);
    }
}
