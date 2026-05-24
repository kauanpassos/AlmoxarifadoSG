using MediatR;
using Almoxarifado.Domain.Interfaces;
using Almoxarifado.Application.Queries;
using Almoxarifado.Domain.Entities;
namespace Almoxarifado.Application.Handlers;
public sealed class GetEstoqueHandler(IReadOnlyRepository<Estoque> repository) : IRequestHandler<GetEstoqueQuery, IEnumerable<Estoque>>
{
    public async Task<IEnumerable<Estoque>> Handle(GetEstoqueQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetAllAsync();
    }
}
