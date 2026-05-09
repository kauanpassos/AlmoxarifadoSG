using MediatR;
using Almoxarifado.Domain;

namespace Almoxarifado.Application.Commands;

// Comando para registrar uma nova solicitação no almoxarifado.
// Substitui o antigo 'Pedido' para alinhar com a linguagem ubíqua do domínio.
public record CreateSolicitacaoCommand(Solicitacao Solicitacao) : IRequest<Solicitacao>;
