using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Almoxarifado.Domain.Interfaces;
using Almoxarifado.Domain.Entities;
using Almoxarifado.Application.DTOs;
using Almoxarifado.Application.Queries;

namespace Almoxarifado.Application.Handlers;

public sealed class GetSolicitacoesPendentesHandler(IReadOnlyRepository<Solicitacao> repository)
    : IRequestHandler<GetSolicitacoesPendentesQuery, IEnumerable<SolicitacaoDto>>
{
    public async Task<IEnumerable<SolicitacaoDto>> Handle(GetSolicitacoesPendentesQuery request, CancellationToken cancellationToken)
    {
        var pendentes = await repository.GetByFieldAsync(nameof(Solicitacao.Status), "Pendente");
        
        return (pendentes ?? []).Select(s => s.ToDto());
    }
}
