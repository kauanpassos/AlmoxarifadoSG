using MediatR;
using Almoxarifado.Domain.Entities;
namespace Almoxarifado.Application.Queries;
public record GetTodasSolicitacoesQuery() : IRequest<IEnumerable<Solicitacao>>;
