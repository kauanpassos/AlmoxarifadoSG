using MediatR;
using Pedido.Application.Commands;
using Pedido.Application.DTOs;
using Pedido.Application.Interfaces;
using Pedido.Domain.Entities;
using Pedido.Domain.Interfaces;

namespace Pedido.Application.Handlers;

// Handler parcial para o microserviço de pedidos.
// A separação por classe parcial isola a configuração de injeção (neste arquivo) da implementação lógica (.Logic.cs).
public sealed partial class CreateOrderHandler(IWriteOnlyRepository<Order> repository, IEstoqueService estoqueService) 
    : IRequestHandler<CreateOrderCommand, OrderDto>;
