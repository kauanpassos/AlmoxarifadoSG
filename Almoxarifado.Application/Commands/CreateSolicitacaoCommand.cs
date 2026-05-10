using MediatR;
using Almoxarifado.Domain;

namespace Almoxarifado.Application.Commands;

// Comando para registrar uma nova solicitação no almoxarifado.
public record CreateSolicitacaoCommand(Solicitacao Solicitacao) : IRequest<Solicitacao>;