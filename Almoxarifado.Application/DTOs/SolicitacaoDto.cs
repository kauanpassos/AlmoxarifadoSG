using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Almoxarifado.Application.DTOs;

public record ItemSolicitacaoDto(string Sku, int Quantidade, string NomeProduto);

public record SolicitacaoDto(
    string Id,
    string UsuarioId,
    string Observacao,
    string Status,
    DateTime CreatedAt,
    IReadOnlyCollection<ItemSolicitacaoDto> Itens);