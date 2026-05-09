using MediatR;
using Almoxarifado.Domain;
using Almoxarifado.Domain.Interfaces;
using Almoxarifado.Application.Queries;

namespace Almoxarifado.Application.Handlers;

// Handler para consulta de pedidos filtrada por usuário.
public sealed class GetPedidosByUserHandler(IReadOnlyRepository<Pedido> repository) : IRequestHandler<GetPedidosByUserQuery, IEnumerable<Pedido>>
{
    public async Task<IEnumerable<Pedido>> Handle(GetPedidosByUserQuery request, CancellationToken cancellationToken)
    {
        // Utilizamos o método especializado de busca por usuário.
        // DICA PARA OS JUNIORES: Centralizem filtros comuns no repositório para evitar duplicar lógica de filtro no código.
        return await repository.GetByUserIdAsync(request.UserId);
    }
}
