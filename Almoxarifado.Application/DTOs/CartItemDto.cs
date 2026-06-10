namespace Almoxarifado.Application.DTOs;

public record CartItemDto(
    string Sku,
    int Quantidade,
    string NomeProduto);