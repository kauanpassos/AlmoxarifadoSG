using MediatR;
using Almoxarifado.Domain;

namespace Almoxarifado.Application.Queries;

public record GetEstoqueQuery() : IRequest<IEnumerable<Estoque>>;
