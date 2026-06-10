using System;

namespace Almoxarifado.Application.DTOs;

public record ProdutoDto(
    string Id,
    string Nome,
    long NumCode,
    long QtdEstoque,
    long EstoqueMinimo,
    DateTime UpdatedAt);