using MediatR;
using Almoxarifado.Domain;

namespace Almoxarifado.Application.Queries;

public record GetPedidosByUserQuery(int UserId) : IRequest<IEnumerable<Pedido>>;
