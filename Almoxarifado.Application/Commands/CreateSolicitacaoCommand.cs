using MediatR;
using Almoxarifado.Domain.Entities;

namespace Almoxarifado.Application.Commands;
public record CreateSolicitacaoCommand(Solicitacao Solicitacao) : IRequest<Solicitacao>;