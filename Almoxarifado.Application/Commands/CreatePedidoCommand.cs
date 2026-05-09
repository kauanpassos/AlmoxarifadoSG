using MediatR;
using Almoxarifado.Domain;

namespace Almoxarifado.Application.Commands;

public record CreatePedidoCommand(Pedido Pedido) : IRequest<Pedido>;
