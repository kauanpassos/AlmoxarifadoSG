using MediatR;
using System.Collections.Generic;

namespace Almoxarifado.Application.Commands;
public record SolicitacaoItemRequest(string Sku, string NomeProduto, int Quantidade);

public record CreateSolicitacaoCommand(
    string UsuarioId,
    string Observacao,
    List<SolicitacaoItemRequest> Itens) : IRequest<Almoxarifado.Application.DTOs.SolicitacaoDto>;