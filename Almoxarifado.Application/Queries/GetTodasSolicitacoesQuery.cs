using MediatR;
using Almoxarifado.Domain;
namespace Almoxarifado.Application.Queries;
public record GetTodasSolicitacoesQuery() : IRequest<IEnumerable<Solicitacao>>;
