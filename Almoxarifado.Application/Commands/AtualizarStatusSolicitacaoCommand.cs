using MediatR;
using Almoxarifado.Domain;
namespace Almoxarifado.Application.Commands;
public record AtualizarStatusSolicitacaoCommand(string SolicitacaoId, string NovoStatus) : IRequest<Solicitacao>;
