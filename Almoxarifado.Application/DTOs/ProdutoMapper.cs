using Almoxarifado.Application.DTOs;
using Almoxarifado.Domain.Entities;

namespace Almoxarifado.Application.DTOs;

public static class ProdutoMapper
{
    public static ProdutoDto ToDto(this Produto entity)
    {
        return new ProdutoDto(
            entity.Id,
            entity.Nome,
            entity.NumCode,
            entity.QtdEstoque,
            entity.EstoqueMinimo,
            entity.UpdatedAt
        );
    }
}
