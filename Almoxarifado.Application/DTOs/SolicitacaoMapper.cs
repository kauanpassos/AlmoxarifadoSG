using Almoxarifado.Domain.Entities;
using System.Linq;

namespace Almoxarifado.Application.DTOs;

public static class SolicitacaoMapper
{
    public static SolicitacaoDto ToDto(this Solicitacao entity)
    {
        return new SolicitacaoDto(
            entity.Id,
            entity.UsuarioId,
            entity.Observacao,
            entity.Status,
            entity.CreatedAt,
            entity.Itens.Select(i => new ItemSolicitacaoDto(i.ProdutoId, i.Quantidade, i.NomeProduto)).ToList().AsReadOnly()
        );
    }
}
