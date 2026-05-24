using MediatR;
using Almoxarifado.Domain.Entities;
namespace Almoxarifado.Application.Commands;
public record AtualizarStatusSolicitacaoCommand(string SolicitacaoId, string NovoStatus) : IRequest<Solicitacao>;
