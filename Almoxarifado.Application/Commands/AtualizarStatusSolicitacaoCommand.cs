using MediatR;
using Almoxarifado.Application.DTOs;

namespace Almoxarifado.Application.Commands;
public record AtualizarStatusSolicitacaoCommand(string Id, string NovoStatus) : IRequest<SolicitacaoDto>;
