using MediatR;
using Almoxarifado.Domain.Interfaces;
using Almoxarifado.Application.Queries;
using Almoxarifado.Domain.Entities;
using Almoxarifado.Application.DTOs;

using System.Linq;

namespace Almoxarifado.Application.Handlers;
public sealed class GetTodasSolicitacoesHandler(IReadOnlyRepository<Solicitacao> repository) 
    : IRequestHandler<GetTodasSolicitacoesQuery, IEnumerable<SolicitacaoDto>>
{
    public async Task<IEnumerable<SolicitacaoDto>> Handle(GetTodasSolicitacoesQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetAllAsync();
        return (result ?? []).Select(s => s.ToDto());
    }
}
