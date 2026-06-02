using MediatR;
using Almoxarifado.Domain.Interfaces;
using Almoxarifado.Application.Queries;
using Almoxarifado.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Almoxarifado.Application.Handlers;

// Trocamos todas as menções de Estoque por Produto
public sealed class GetEstoqueHandler(IReadOnlyRepository<Produto> repository) : IRequestHandler<GetEstoqueQuery, IEnumerable<Produto>>
{
    public async Task<IEnumerable<Produto>> Handle(GetEstoqueQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetAllAsync();
    }
}