using System;
using System.Collections.Generic;

namespace Almoxarifado.Application.DTOs;

public record ItemSolicitacaoDto(string Sku, int Quantidade);

public record SolicitacaoDto(
    string Id,
    string UsuarioId,
    string Observacao,
    string Status,
    DateTime CreatedAt,
    IReadOnlyCollection<ItemSolicitacaoDto> Itens);
