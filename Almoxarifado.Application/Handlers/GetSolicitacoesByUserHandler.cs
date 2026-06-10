using MediatR;
using Almoxarifado.Domain.Interfaces;
using Almoxarifado.Application.Queries;
using Almoxarifado.Domain.Entities;
using Almoxarifado.Application.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Almoxarifado.Application.Handlers;

public sealed class GetSolicitacoesByUserHandler(IReadOnlyRepository<Solicitacao> repository)
    : IRequestHandler<GetSolicitacoesByUserQuery, IEnumerable<SolicitacaoDto>>
{
    public async Task<IEnumerable<SolicitacaoDto>> Handle(GetSolicitacoesByUserQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetByFieldAsync("UsuarioId", request.UsuarioId);

        return (result ?? []).Select(s => s.ToDto());
    }
}