using MediatR;
using Almoxarifado.Domain.Entities;

using System.Collections.Generic;

namespace Almoxarifado.Application.Commands;

public record SolicitacaoItemRequest(string Sku, int Quantidade);

public record CreateSolicitacaoCommand(
    string UsuarioId, 
    string Observacao, 
    List<SolicitacaoItemRequest> Itens) : IRequest<Almoxarifado.Application.DTOs.SolicitacaoDto>;